using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.RegisterSubjectService.Models
{
    public class RegisterSubjectResponse
    {
        public int RegisterSubjectId { get; set; }
        public int UserId { get; set; }
        public int AvailableSubjectId { get; set; }
        public int ClassId { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool Status { get; set; }
        public string SubjectName { get; set; }

    }
}
