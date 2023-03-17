using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamService.Models
{
    public class ExamUpdateRequestModel
    {

        public string ExamContent { get; set; } = null!;
        public string? ExamLink { get; set; } = null!;
    }
}
