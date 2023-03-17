using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int LeaderId { get; set; }
        public string? CommentContent { get; set; }
        public int ExamPaperId { get; set; }

        public virtual ExamPaper ExamPaper { get; set; } = null!;
    }
}
