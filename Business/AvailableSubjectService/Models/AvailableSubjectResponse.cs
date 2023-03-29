using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AvailableSubjectService.Models
{
    public class AvailableSubjectResponse
    {
        public int AvailableSubjectId { get; set; }
        public int ExamScheduleId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int LeaderId { get; set; }
        public string? SubjectName { get; set; }
        public string? LeaderName { get; set; }
        public string? TypeName { get; set; }
    }
}
