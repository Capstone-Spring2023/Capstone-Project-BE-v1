using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class RegisterDeadline
    {
        public int Id { get; set; }
        public DateTime Deadline { get; set; }
        public int SemesterId { get; set; }

        public virtual Semester Semester { get; set; } = null!;
    }
}
