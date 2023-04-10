using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class ExamSchedule
    {
        public ExamSchedule()
        {
            ExamPapers = new HashSet<ExamPaper>();
        }

        public int ExamScheduleId { get; set; }
        public int RegisterSubjectId { get; set; }
        public DateTime Deadline { get; set; }
        public int TypeId { get; set; }
        public int AvailableSubjectId { get; set; }
        public int LeaderId { get; set; }
        public string? Tittle { get; set; }
        public string? ExamLink { get; set; }
        public bool Status { get; set; }
        public int? AppovalUserId { get; set; }

        public virtual RegisterSubject RegisterSubject { get; set; } = null!;
        public virtual ICollection<ExamPaper> ExamPapers { get; set; }
    }
}
