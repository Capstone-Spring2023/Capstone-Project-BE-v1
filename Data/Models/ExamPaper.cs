using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class ExamPaper
    {
        public ExamPaper()
        {
            Comments = new HashSet<Comment>();
        }

        public int ExamPaperId { get; set; }
        public int ExamScheduleId { get; set; }
        public string? ExamContent { get; set; }
        public string? ExamLink { get; set; }
        public string? ExamInstruction { get; set; }
        public string Status { get; set; } = null!;

        public virtual ExamSchedule ExamSchedule { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
