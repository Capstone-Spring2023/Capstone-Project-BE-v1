using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamSchedule.Models
{
    public class ResponseGroupExamSchedule
    {
        public DateTime Deadline { get; set; }
        public int AvailableSubjectId { get; set; }
        public int LeaderId { get; set; }
        public string? SubjectName { get; set; }
        public string? Tittle { get; set; }
        public string? ExamLink { get; set; }
    }
}
