using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatApplication.DBContext;
using Chat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Chat.Helper;

namespace Chat.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ChatContext _context;
        private readonly EncryptionHelper _encryptionHelper;
        private readonly IWebHostEnvironment _webHost;

        public ChatController(ChatContext context, IWebHostEnvironment webHost, EncryptionHelper encryptionHelper)
        {
            _context = context;
            _webHost = webHost;
            _encryptionHelper = encryptionHelper;
        }

        public IActionResult ChatWithUser(int chatFriendId)
        {
            var currentUserId = int.Parse(User.FindFirst("UserID").Value);

            var isfriend = _context.FriendLists.Any(f => (f.UserId == currentUserId && f.FriendUserId == chatFriendId) || (f.UserId == chatFriendId && f.FriendUserId == currentUserId));

            if (!isfriend) return RedirectToAction("Dashboard", "Dashboard");

            var friend = _context.Users.FirstOrDefault(u => u.UserId == chatFriendId);

            if (friend == null)
                return NotFound();

            ViewBag.FriendName = friend.Username;
            ViewBag.FriendId = friend.UserId;

            return View("ChatWithUser");
        }

        [HttpGet]
        public IActionResult GetMessages(int userId)
        {
            int currentUserId = int.Parse(User.FindFirstValue("UserID"));

            var datetime = _context.FriendLists.FirstOrDefault(m =>
                    (m.UserId == currentUserId && m.FriendUserId == userId) ||
                    (m.UserId == userId && m.FriendUserId == currentUserId));

            if (datetime == null)
            {
                return Json(new List<object>());
            }

            var messages = _context.Messages
                .Where(m =>
                    (m.FromUserId == currentUserId && m.ToUserId == userId && m.SentAt >= datetime.CreatedDate) ||
                    (m.FromUserId == userId && m.ToUserId == currentUserId && m.SentAt >= datetime.CreatedDate))
                .OrderBy(m => m.SentAt)
                .Select(m => new {
                    MessageText = _encryptionHelper.Decrypt(m.MessageText),
                    m.FileUrl,
                    m.SentAt,
                    m.FromUserId
                })
                .ToList();

            return Json(messages);
        }

        public IActionResult ChatWithGroup(int groupId)
        {
            var group = _context.GroupLists.FirstOrDefault(g => g.GroupId == groupId);
            if (group == null)
                return NotFound();

            ViewBag.GroupName = group.GroupName;
            ViewBag.GroupId = group.GroupId;

            return View("GroupChat"); 
        }

        [HttpGet]
        public IActionResult GetGroupMessages(int groupId)
        {
            int currentUserId = int.Parse(User.FindFirstValue("UserID"));

            var datetime = _context.GroupMembers.FirstOrDefault(m =>
                    (m.UserId == currentUserId && m.GroupId == groupId));

            var messages = _context.Messages
                .Include(m => m.FromUser) 
                .Where(m => m.GroupId == groupId && m.SentAt >= datetime.CreatedDate)
                .OrderBy(m => m.SentAt)
                .Select(m => new {
                    MessageText = _encryptionHelper.Decrypt(m.MessageText),
                    m.FileUrl,
                    m.SentAt,
                    m.FromUserId,
                    FromUsername = m.FromUser.Username
                })
                .ToList();

            return Json(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file selected" });

            if (file.Length > 1024 * 1024 * 1024)
                return Json(new { success = false, message = "File exceeds limit" });

            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            string fileUrl = "/uploads/" + fileName;
            return Json(new { success = true, url = fileUrl });
        }
    }
}


