using System;
using System.Collections.Generic;

namespace ChatApplication.DBContext;

public partial class GroupMember
{
    public int Id { get; set; }

    public int? GroupId { get; set; }

    public int? UserId { get; set; }

    public bool? IsAdmin { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual GroupList? Group { get; set; }

    public virtual User? User { get; set; }
}
