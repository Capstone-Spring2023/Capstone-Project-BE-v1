using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamService.Models
{
    public class ExamResponseModel
    {
        public int ExamPaperId { get; set; }
        public int ExamScheduleId { get; set; }
        public string? ExamContent { get; set; }
        public string? ExamLink { get; set; }
        public string Type { get; set; }
        public string? ExamInstruction { get; set; }
        public string SubjectName { get; set; }
        public string LecturerName { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; } 
        public string Tittle { get; set; }

    }
}
