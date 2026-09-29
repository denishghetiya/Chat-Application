using System;
using System.Collections.Generic;

namespace ChatApplication.DBContext;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? ImageName { get; set; }

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpiry { get; set; }

    public bool IsOnline { get; set; }

    public virtual ICollection<FriendList> FriendListFriendUsers { get; set; } = new List<FriendList>();

    public virtual ICollection<FriendList> FriendListUsers { get; set; } = new List<FriendList>();

    public virtual ICollection<FriendRequest> FriendRequestFromUsers { get; set; } = new List<FriendRequest>();

    public virtual ICollection<FriendRequest> FriendRequestToUsers { get; set; } = new List<FriendRequest>();

    public virtual ICollection<GroupList> GroupLists { get; set; } = new List<GroupList>();

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Message> MessageFromUsers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageToUsers { get; set; } = new List<Message>();
}
