using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Department
    {
        public Department()
        {
            CurrentHeaders = new HashSet<CurrentHeader>();
            Subjects = new HashSet<Subject>();
        }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public bool Status { get; set; }

        public virtual ICollection<CurrentHeader> CurrentHeaders { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
