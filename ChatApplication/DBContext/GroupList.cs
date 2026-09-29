using System;
using System.Collections.Generic;

namespace ChatApplication.DBContext;

public partial class GroupList
{
    public int GroupId { get; set; }

    public string? GroupName { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
