using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class CurrentHeader
    {
        public int HeaderId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public int SemesterId { get; set; }
        public bool Status { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual Semester Semester { get; set; } = null!;
    }
}
