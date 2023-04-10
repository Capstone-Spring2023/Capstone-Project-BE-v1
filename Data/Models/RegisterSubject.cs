using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class RegisterSubject
    {
        public RegisterSubject()
        {
            Classes = new HashSet<Class>();
            ExamSchedules = new HashSet<ExamSchedule>();
        }

        public int RegisterSubjectId { get; set; }
        public int UserId { get; set; }
        public int AvailableSubjectId { get; set; }
        public int ClassId { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool Status { get; set; }
        public bool? IsRegistered { get; set; }

        public virtual AvailableSubject AvailableSubject { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; }
    }
}
