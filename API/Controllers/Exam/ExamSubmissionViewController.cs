using Business.ExamService.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Exam
{
    [ApiController]
    [Route("api/exam-submission-view")]
    public class ExamSubmissionViewController : ControllerBase
    {

        [HttpPut("review-exam")]
        public async Task<ObjectResult> ReviewExam([FromBody] ReviewExamModel reviewExamModel)
        {
            //var response = await examService.ApproveExam(reviewExamModel.CommentModel, reviewExamModel.ExamUpdateApproveModel);
            return null;
        }
    }
}
