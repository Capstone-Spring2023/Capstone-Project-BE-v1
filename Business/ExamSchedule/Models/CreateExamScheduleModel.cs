using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamSchedule.Models
{
    public class CreateExamScheduleModel
    {
        public string? Tittle { get; set; }
        public DateTime Deadline { get; set; }
        public int AppovalUserId { get; set; }
        public string? ExamLink { get; set; }
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int LeaderId { get; set; }

    }
    public class UpdateExamScheduleModel
    {
        [Required]
        public string? Tittle { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
        [Required]
        public string? ExamLink { get; set; }
    }
}
