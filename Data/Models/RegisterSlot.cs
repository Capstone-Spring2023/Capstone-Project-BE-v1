using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class RegisterSlot
    {
        public int RegisterSlotId { get; set; }
        public string? Slot { get; set; }
        public bool Status { get; set; }
        public int SemesterId { get; set; }
        public int UserId { get; set; }

        public virtual Semester Semester { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
