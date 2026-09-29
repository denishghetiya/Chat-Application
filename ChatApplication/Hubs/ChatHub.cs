using ChatApplication.DBContext;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Chat.Helper;

public static class OnlineUserTracker
{
    public static ConcurrentDictionary<int, bool> OnlineUsers = new ConcurrentDictionary<int, bool>();
}

public class ChatHub : Hub
{
    private readonly ChatContext _context;
    private readonly EncryptionHelper _encryptionHelper;
    private static HashSet<int> OnlineUserIds = new();
    public static Dictionary<string, string> ConnectedUsers = new();

    public ChatHub(ChatContext context, EncryptionHelper encryptionHelper)
    {
        _context = context;
        _encryptionHelper = encryptionHelper;
    }

    public override async Task OnConnectedAsync()
    {
        string userIdStr = Context.User?.FindFirst("UserID")?.Value;

        if (int.TryParse(userIdStr, out int userId))
        {
            OnlineUserTracker.OnlineUsers[userId] = true;

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = true;
                await _context.SaveChangesAsync();
            }

            await Clients.All.SendAsync("UserStatusChanged", userId, true);

        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string userIdStr = Context?.User?.FindFirst("UserID")?.Value;

        if (int.TryParse(userIdStr, out int userId))
        {
            OnlineUserTracker.OnlineUsers.TryRemove(userId, out _);

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = false;
                await _context.SaveChangesAsync();
            }

            await Clients.All.SendAsync("UserStatusChanged", userId, false);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string toUserId, string message)
    {
        var fromUserId = Context.User.FindFirst("UserID")?.Value;

        if (string.IsNullOrEmpty(fromUserId) || string.IsNullOrEmpty(toUserId) || string.IsNullOrEmpty(message) )
            return;

        if (message != null)
        {
            var encryptedMessage = _encryptionHelper.Encrypt(message);

            var newMessage = new Message
            {
                FromUserId = int.Parse(fromUserId),
                ToUserId = int.Parse(toUserId),
                MessageText = encryptedMessage,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            await Clients.User(toUserId).SendAsync("ReceiveMessage", fromUserId, message);
            await Clients.User(fromUserId).SendAsync("ReceiveMessage", fromUserId, message);
        }
    }

    public async Task Typing(string toUser)
    {
        await Clients.User(toUser).SendAsync("UserTyping", Context.User.Identity.Name);
    }

    public async Task CheckUserStatus(int userId)
    {
        bool isOnline = OnlineUserTracker.OnlineUsers.TryGetValue(userId, out bool online) && online;
        await Clients.Caller.SendAsync("ReceiveUserStatus", userId, isOnline);
    }

    public async Task SendGroupMessage(string groupId, string groupName, string message)
    {
        int fromUserId = int.Parse(Context.User.FindFirst("UserID").Value);
        string fromUsername = Context.User.Identity.Name;

        //var group = await _context.GroupLists.FirstOrDefaultAsync(g => g.GroupId == int.Parse(groupId));

        //if (group == null)
        //{
        //    await Clients.Caller.SendAsync("ReceiveSystemMessageGroup", "Error: Group not found.");
        //    return;
        //}

        //var isMember = await _context.GroupMembers.AnyAsync(gm => gm.GroupId == int.Parse(groupId) && gm.UserId == fromUserId);
        //if (!isMember)
        //{
        //    await Clients.Caller.SendAsync("ReceiveSystemMessageGroup", "Error: You are not a member of this group.");
        //    return;
        //}

        var encryptedMessage = _encryptionHelper.Encrypt(message);
        var msg = new Message
        {
            FromUserId = fromUserId,
            GroupId = int.Parse(groupId),
            MessageText = encryptedMessage,
            SentAt = DateTime.Now
        };

        _context.Messages.Add(msg);
        await _context.SaveChangesAsync();

        await Clients.Group(groupId).SendAsync("ReceiveGroupMessage", fromUserId, fromUsername, message, groupId);
    }

    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

}
