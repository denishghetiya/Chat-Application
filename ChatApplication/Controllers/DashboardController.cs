using Chat.ViewModels;
using ChatApplication.DBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Chat.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ChatContext _context;
        private readonly IWebHostEnvironment _webHost;
        private readonly IHubContext<ChatHub> _hubContext;

        public DashboardController(ChatContext context, IWebHostEnvironment webHost, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _webHost = webHost;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            int userId = int.Parse(User.FindFirst("UserID").Value);
            var user = await _context.Users.FirstOrDefaultAsync(f => f.UserId == userId);
            ViewBag.Username = user.Username;
            return View("Dashboard");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(f => f.UserId == id);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
                ExistingImagePath = user.ImageName != null ? $"/Uploads/{user.ImageName}" : null
            };
            return View("Edit", model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            var user = await _context.Users.FirstOrDefaultAsync(f => f.UserId == model.UserId);
            if (user == null) return NotFound();

            var email = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (email != null)
            {
                if (model.Email != user.Email && model.Email == email.Email)
                {
                    ModelState.AddModelError("", "This Email has already account.");
                    return View(model);
                }
            }

            if (email == null)
            {
                user.Email = model.Email;
            }

            user.Username = model.Username;
            user.Password = model.Password;

            if (model.Image != null)
            {
                var uploadsFolder = Path.Combine(_webHost.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(user.ImageName))
                {
                    var oldImagePath = Path.Combine(uploadsFolder, user.ImageName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var originalFileName = Path.GetFileNameWithoutExtension(model.Image.FileName);
                var extension = Path.GetExtension(model.Image.FileName);
                var uniqueFileName = $"{originalFileName}_{timeStamp}{extension}";

                var fileSavePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                user.ImageName = uniqueFileName;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue("UserID"));
        }

        public IActionResult Search(string email)
        {
            int currentUserId = GetCurrentUserId();

            var users = _context.Users
                .Where(u => u.Email.Contains(email) && u.UserId != currentUserId)
                .Select(u => new
                {
                    u.UserId,
                    u.Email,
                    IsFriend = _context.FriendLists.Any(f =>
                        (f.UserId == currentUserId && f.FriendUserId == u.UserId)
                        || (f.UserId == u.UserId && f.FriendUserId == currentUserId)
                        ),

                    IsPending = _context.FriendRequests.Any(r =>
                        (r.FromUserId == currentUserId && r.ToUserId == u.UserId && r.Status == "Pending") || (r.FromUserId == u.UserId && r.ToUserId == currentUserId && r.Status == "Pending"))
                })
                .ToList();

            var result = users.Select(u => new
            {
                u.UserId,
                u.Email,
                Action = u.IsPending ? "pending" : (u.IsFriend ? "remove" : "add")
            });

            return Json(result);
        }

        [HttpPost]
        public IActionResult SendRequest(int toUserId)
        {
            var fromUserId = GetCurrentUserId();

            var alreadyRequested = _context.FriendRequests.Any(fr =>
                (fr.FromUserId == fromUserId && fr.ToUserId == toUserId) || (fr.FromUserId == toUserId && fr.ToUserId == fromUserId));

            if (!alreadyRequested)
            {
                _context.FriendRequests.Add(new FriendRequest
                {
                    FromUserId = fromUserId,
                    ToUserId = toUserId,
                    Status = "Pending"
                });
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest("Already requested");
        }

        [HttpPost]
        public IActionResult AcceptRequest(int requestId)
        {
            var fromUserId = GetCurrentUserId();

            var request = _context.FriendRequests.FirstOrDefault(r => r.RequestId == requestId && r.Status == "Pending");

            _context.FriendLists.AddRange(
                new FriendList { UserId = request.FromUserId, FriendUserId = request.ToUserId, CreatedDate = DateTime.Now }
            );
            _context.FriendRequests.Remove(request);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult RemoveFriend(int? friendUserId, int? requestId)
        {
            if (friendUserId != null)
            {
                int currentUserId = GetCurrentUserId();

                var friendship = _context.FriendLists.FirstOrDefault(f =>
                    (f.UserId == currentUserId && f.FriendUserId == friendUserId) || (f.UserId == friendUserId && f.FriendUserId == currentUserId));

                var messages = _context.Messages.Where(g => g.FromUserId == currentUserId && g.ToUserId == friendUserId || g.FromUserId == friendUserId && g.ToUserId == currentUserId).ToList();

                if (friendship != null)
                {
                    _context.FriendLists.Remove(friendship);
                    //_context.Messages.RemoveRange(messages);
                    _context.SaveChanges();
                    _hubContext.Clients.User(friendUserId.ToString()).SendAsync("FriendRemoved", currentUserId);
                }

                return Ok();
            }
            if (requestId != null)
            {
                var request = _context.FriendRequests.FirstOrDefault(f => f.RequestId == requestId.Value);

                if (request != null)
                {
                    _context.FriendRequests.Remove(request);
                    _context.SaveChanges();
                }

                return Ok();
            }
            return Ok();
        }

        public IActionResult FriendList(string? username)
        {
            var currentUserId = GetCurrentUserId();

            var friends = _context.FriendLists
                .Include(f => f.User)
                .Include(f => f.FriendUser)
                .Where(f => f.UserId == currentUserId || f.FriendUserId == currentUserId)
                .Select(f => new
                {
                    UserId = f.UserId == currentUserId ? f.FriendUser.UserId : f.User.UserId,
                    Username = f.UserId == currentUserId ? f.FriendUser.Username : f.User.Username,
                    IsOnline = f.UserId == currentUserId ? f.FriendUser.IsOnline : f.User.IsOnline
                });

            if (!string.IsNullOrWhiteSpace(username))
            {
                friends = friends.Where(f => f.Username.Contains(username));
            }

            return Json(friends.ToList());
        }

        public IActionResult GetPendingRequests()
        {
            var currentUserId = GetCurrentUserId();

            var requests = _context.FriendRequests
                .Where(fr => fr.ToUserId == currentUserId && fr.Status == "Pending")
                .Include(fr => fr.FromUser)
                .Select(fr => new
                {
                    requestId = fr.RequestId,
                    fromUsername = fr.FromUser.Username,
                    fromEmail = fr.FromUser.Email,
                    status = fr.Status
                })
                .ToList();

            return Json(requests);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyGroups()
        {
            int currentUserId = GetCurrentUserId();

            var groups = await _context.GroupMembers
                .Where(gm => gm.UserId == currentUserId)
                .Select(gm => new
                {
                    gm.GroupId,
                    gm.Group.GroupName
                })
                .ToListAsync();

            return Json(groups);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int currentUserId = GetCurrentUserId();

            if (await _context.GroupLists.AnyAsync(g => g.GroupName == model.GroupName))
            {
                return BadRequest(new { message = "Group with this name already exists." });
            }

            var newGroup = new GroupList
            {
                GroupName = model.GroupName,
                CreatedBy = currentUserId,
                CreatedDate = DateTime.Now
            };
            _context.GroupLists.Add(newGroup);
            await _context.SaveChangesAsync();

            var creatorMember = new GroupMember { GroupId = newGroup.GroupId, UserId = currentUserId, IsAdmin = true, CreatedDate = DateTime.Now };
            _context.GroupMembers.Add(creatorMember);

            foreach (var userId in model.MemberUserIds)
            {
                if (userId != currentUserId)
                {
                    var member = new GroupMember { GroupId = newGroup.GroupId, UserId = userId, IsAdmin = false, CreatedDate = DateTime.Now };
                    _context.GroupMembers.Add(member);
                }
            }
            await _context.SaveChangesAsync();

            foreach (var userId in model.MemberUserIds.Concat(new[] { currentUserId }).Distinct())
            {
                await _hubContext.Clients.User(userId.ToString()).SendAsync("NewGroupCreated", newGroup.GroupId, newGroup.GroupName);
            }

            return Ok(new { groupId = newGroup.GroupId, groupName = newGroup.GroupName, message = $"Group '{model.GroupName}' created successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupMembers(int groupId)
        {
            int currentUserId = GetCurrentUserId();

            var members = await _context.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Select(gm => new { gm.UserId, gm.User.Username, gm.User.Email, gm.User.IsOnline, gm.IsAdmin })
                .ToListAsync();

            var groupAdmin = await _context.GroupMembers
                .Where(gm => gm.GroupId == groupId && gm.UserId == currentUserId && gm.IsAdmin == true)
                .Select(gm => new { gm.UserId, gm.User.Username, gm.User.Email, gm.User.IsOnline, gm.IsAdmin })
                .FirstOrDefaultAsync();

            return Json(new
            {
                members = members,
                groupAdmin = groupAdmin
            });
        }
        [HttpGet]
        public async Task<IActionResult> FriendNotInGroup(string? username, int groupId)
        {
            var currentUserId = GetCurrentUserId();

            var groupMemberIds = await _context.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Select(gm => gm.UserId)
                .ToListAsync();

            var friends = _context.FriendLists
                .Include(f => f.User)
                .Include(f => f.FriendUser)
                .Where(f => f.UserId == currentUserId || f.FriendUserId == currentUserId)
                .Select(f => new
                {
                    UserId = f.UserId == currentUserId ? f.FriendUser.UserId : f.User.UserId,
                    Username = f.UserId == currentUserId ? f.FriendUser.Username : f.User.Username,
                    IsOnline = f.UserId == currentUserId ? f.FriendUser.IsOnline : f.User.IsOnline
                })
                .Where(f => !groupMemberIds.Contains(f.UserId));

            if (!string.IsNullOrWhiteSpace(username))
            {
                friends = friends.Where(f => f.Username.Contains(username));
            }

            return Json(await friends.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> RemoveGroupMember(int groupId, int userIdToRemove)
        {
            int currentUserId = GetCurrentUserId();
            var group = await _context.GroupLists.FirstOrDefaultAsync(g => g.GroupId == groupId);
            var groupMember = await _context.GroupMembers.FirstOrDefaultAsync(g => g.GroupId == groupId && g.UserId == currentUserId);

            if (group == null || groupMember.IsAdmin != true)
            {
                return Forbid("You don't have permission to remove members from this group.");
            }

            if (currentUserId == userIdToRemove)
            {
                return BadRequest("You cannot remove yourself using this function. Use 'Leave Group' instead.");
            }

            var memberToRemove = await _context.GroupMembers.FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userIdToRemove);

            if (memberToRemove == null)
            {
                return NotFound("User is not a member of this group.");
            }

            _context.GroupMembers.Remove(memberToRemove);
            await _context.SaveChangesAsync();

            var removedUser = await _context.Users.FindAsync(userIdToRemove);
            await _hubContext.Clients.User(userIdToRemove.ToString()).SendAsync("YouWereRemovedFromGroup", groupId, group.GroupName);
            await _hubContext.Clients.Group(group.GroupName).SendAsync("ReceiveSystemMessage", $"{removedUser?.Username ?? "A user"} was removed from the group.");

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddMemberToGroup(int groupId, int userIdToAdd)
        {
            int currentUserId = GetCurrentUserId();
            var group = await _context.GroupLists.FirstOrDefaultAsync(g => g.GroupId == groupId);
            var groupMember = await _context.GroupMembers.FirstOrDefaultAsync(g => g.GroupId == groupId && g.UserId == currentUserId);

            if (group == null || groupMember.IsAdmin != true)
            {
                return Forbid("You don't have permission to add members to this group.");
            }

            var userToAdd = await _context.Users.FindAsync(userIdToAdd);
            if (userToAdd == null)
            {
                return NotFound("User to add not found.");
            }

            var newMember = new GroupMember
            {
                GroupId = groupId,
                UserId = userIdToAdd,
                CreatedDate = DateTime.Now
            };
            _context.GroupMembers.Add(newMember);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(userIdToAdd.ToString()).SendAsync("AddedToGroup", groupId, group.GroupName);
            await _hubContext.Clients.Group(group.GroupName).SendAsync("ReceiveSystemMessage", $"{userToAdd.Username} has been added to the group.");

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MakeAdmin(int groupId, int userId)
        {
            var groupMember = await _context.GroupMembers.FirstOrDefaultAsync(g => g.GroupId == groupId && g.UserId == userId);
            groupMember.IsAdmin = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(int groupId, int userId)
        {
            var groupMember = await _context.GroupMembers.FirstOrDefaultAsync(g => g.GroupId == groupId && g.UserId == userId);
            groupMember.IsAdmin = false;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LeaveGroup(int groupId, string groupName)
        {
            int userId = GetCurrentUserId();

            var group = await _context.GroupLists.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group != null)
            {
                var member = await _context.GroupMembers.FirstOrDefaultAsync(gm => gm.GroupId == group.GroupId && gm.UserId == userId);

                if (member != null)
                {
                    if (member.IsAdmin == true)
                    {
                        var hasOtherAdmin = _context.GroupMembers.Any(gn => gn.GroupId == group.GroupId && gn.IsAdmin == true && gn.UserId != userId);

                        if (!hasOtherAdmin)
                        {
                            var makeAdmin = await _context.GroupMembers.FirstOrDefaultAsync(g => g.GroupId == group.GroupId && g.UserId != userId);
                            if (makeAdmin != null)
                            {
                                makeAdmin.IsAdmin = true;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }

                    _context.GroupMembers.Remove(member);
                    await _context.SaveChangesAsync();

                    //var isanyothermember = _context.GroupMembers.Any(gn => gn.GroupId == group.GroupId);
                    //if (isanyothermember == false)
                    //{
                    //    var messages = _context.Messages.Where(g => g.GroupId == groupId).ToList();
                    //    _context.GroupLists.Remove(group);
                    //    _context.Messages.RemoveRange(messages);
                    //    await _context.SaveChangesAsync();
                    //}
                }
            }

            await _hubContext.Clients.Group(groupName)
                .SendAsync("ReceiveSystemMessageGroup", $"{User.Identity.Name} left the group.");

            return Ok(new { message = "You have left the group." });
        }

        public async Task<IActionResult> RemoveGroup(int groupId)
        {
            var group = _context.GroupLists.FirstOrDefault(g => g.GroupId == groupId);
            var members = _context.GroupMembers.Where(g => g.GroupId == groupId).ToList();
            var messages = _context.Messages.Where(g => g.GroupId == groupId).ToList();

            _context.GroupLists.Remove(group);
            _context.GroupMembers.RemoveRange(members);
            _context.Messages.RemoveRange(messages);

            _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }
    }
}


