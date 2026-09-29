using System;
using System.Collections.Generic;

namespace ChatApplication.DBContext;

public partial class FriendRequest
{
    public int RequestId { get; set; }

    public int? FromUserId { get; set; }

    public int? ToUserId { get; set; }

    public string? Status { get; set; }

    public DateTime? RequestedAt { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual User? ToUser { get; set; }
}
