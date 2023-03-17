using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
