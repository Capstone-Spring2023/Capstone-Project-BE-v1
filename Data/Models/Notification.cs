using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Notification
    {
        public int NotiId { get; set; }
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int UserId { get; set; }
        public string? Sender { get; set; }
        public string? SubjectCode { get; set; }
        public string? Status { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
