using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AbleSubjectService.Models
{
    public class AbleSubjectResponse
    {
        public int subjectId { get; set; }
        public string subjectName { get; set; }
    }
    public class AbleSubjectRequest
    {
        public int subjectId { get; set; }
        public int userId { get; set; }
    }
    internal class RequestAndResponse
    {
    }
}
