using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamSchedule.Models
{
    public class StatisticalModelForLecturerOrLeader
    {
        public int totalExamNeedApprove { get; set; }
        public int totalExamNeedSubmit { get; set; }
        public int totalClassTeaching { get; set; }
    }
}
