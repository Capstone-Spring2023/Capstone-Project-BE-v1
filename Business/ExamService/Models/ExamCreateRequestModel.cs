using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamService.Models
{
    public class ExamCreateRequestModel
    {

        
        [Required]
        public string? ExamContent { get; set; }
        [Required]
        public string? ExamLink { get; set; }
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;

    }
    public class ExamUpdateInstructionLinkModel
    {
        public string ExamInstruction { get; set; }
    }
}
