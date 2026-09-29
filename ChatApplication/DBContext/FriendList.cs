using System;
using System.Collections.Generic;

namespace ChatApplication.DBContext;

public partial class FriendList
{
    public int FriendId { get; set; }

    public int? UserId { get; set; }

    public int? FriendUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual User? FriendUser { get; set; }

    public virtual User? User { get; set; }
}
