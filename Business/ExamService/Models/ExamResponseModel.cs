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
        public string ExamContent { get; set; } = null!;
        public string ExamLink { get; set; } = null!;
        public bool IsApproved { get; set; }
    }
}
