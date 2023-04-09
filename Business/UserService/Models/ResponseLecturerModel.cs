using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.UserService.Models
{
    public class ResponseLecturerModel
    {
        public string fullName { get; set; }
        public string semester { get; set; }
        public string subjectName { get; set; }
        public bool isLeader { get; set; }
        public int userId { get; set; }
        public int availableSubjectId { get; set; }
    }
}
