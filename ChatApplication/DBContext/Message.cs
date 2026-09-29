using System;
using System.Collections.Generic;

namespace ChatApplication.DBContext;

public partial class Message
{
    public int MessageId { get; set; }

    public int? FromUserId { get; set; }

    public int? ToUserId { get; set; }

    public int? GroupId { get; set; }

    public string? MessageText { get; set; }

    public string? FileUrl { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual GroupList? Group { get; set; }

    public virtual User? ToUser { get; set; }
}
