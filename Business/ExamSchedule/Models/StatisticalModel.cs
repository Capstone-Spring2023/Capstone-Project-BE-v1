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

    public class StatisticalModelForHeader
    {
        public int totalExamSubmittedOfHeader { get; set; }
        public int totalExamSubmittedOfTeacher { get; set; }
        public int totalClassInSemester { get; set; }
        public int totalExamNeedSubmittedOfTeacher { get; set; }
        public int totalExamNotSubmitOfTeacher { get; set; }
    }
}
