using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Notification
    {
        public int NotiId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
