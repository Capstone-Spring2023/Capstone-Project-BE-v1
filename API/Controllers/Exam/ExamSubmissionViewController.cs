using Business.ExamPaperService.Interfaces;
using Business.ExamService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Exam
{
    [ApiController]
    [Route("api/exam-submission-view")]
    public class ExamSubmissionViewController : ControllerBase
    {
        private readonly IExamPaperService examService;
        public ExamSubmissionViewController(IExamPaperService examService)
        {
            this.examService = examService;
        }

        [HttpPut("review-exam")]
        public async Task<ObjectResult> ReviewExam([FromBody] ReviewExamModel reviewExamModel)
        {
            var response = await examService.ApproveExam(reviewExamModel.CommentModel, reviewExamModel.ExamUpdateApproveModel);
            return response;
        }

        [HttpGet("leader/{leaderId}")]
        public async Task<ObjectResult> ViewExamSubmissionByLeaderId(int leaderId)
        {
            var response = await examService.ViewExamSubmissionByLeaderId(leaderId);
            return response;
        }
        
    }
}
