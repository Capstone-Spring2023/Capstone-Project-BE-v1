using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamService.Models
{
    public class CommentModel
    {
        public int LeaderId { get; set; }
        public int ExamPaperId { get; set; }
        public string? CommentContent { get; set; }
    }
    public class ReviewExamModel
    {
        public CommentModel CommentModel { get; set; }
        public ExamUpdateApproveModel ExamUpdateApproveModel { get; set; }
    }
}
