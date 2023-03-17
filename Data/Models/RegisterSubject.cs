using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class RegisterSubject
    {
        public RegisterSubject()
        {
            ExamSchedules = new HashSet<ExamSchedule>();
        }

        public int RegisterSubjectId { get; set; }
        public int UserId { get; set; }
        public int AvailableSubjectId { get; set; }
        public int ClassId { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool Status { get; set; }

        public virtual AvailableSubject AvailableSubject { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; }
    }
}
