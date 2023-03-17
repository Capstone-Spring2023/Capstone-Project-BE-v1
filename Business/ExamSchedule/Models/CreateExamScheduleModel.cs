using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamSchedule.Models
{
    public class CreateExamScheduleModel
    {
        public string? Tittle { get; set; }
        public DateTime Deadline { get; set; }

        public string? ExamLink { get; set; }
        public int TypeId { get; set; }

    }
}
