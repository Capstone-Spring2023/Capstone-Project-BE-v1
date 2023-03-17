using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Subject
    {
        public Subject()
        {
            AvailableSubjects = new HashSet<AvailableSubject>();
        }

        public int SubjectId { get; set; }
        public int DepartmentId { get; set; }
        public int ExamId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public bool Status { get; set; }
        public string? SubjectName { get; set; }
        public int TypeId { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual Type Type { get; set; } = null!;
        public virtual ICollection<AvailableSubject> AvailableSubjects { get; set; }
    }
}
