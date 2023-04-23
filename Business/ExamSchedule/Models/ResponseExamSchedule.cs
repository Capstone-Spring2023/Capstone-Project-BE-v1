using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamSchedule.Models
{
    public class ResponseExamSchedule
    {
        public int examScheduleId { get; set; }

        public string Tittle { get; set; }
        public DateTime Deadline { get; set; }
        public int AvailableSubjectId { get; set; }
        public string LeaderName { get; set; }
        public string SubjectName { get; set; }
        public string ExamLink { get; set; }
        public int TypeId { get; set; }
        public bool Status { get; set; }
        
    }

    public class ResponseWhoHaveExamAndApprovalUser
    {
        public string ApprovalUserName { get; set; }
        public List<string> Username { get; set; }
        public string SubjectName { get; set; }
    }
}
