using Business;
using Business.ExamPaperService.Interfaces;
using Data.Paging;
using Microsoft.AspNetCore.Mvc;
using Business.ExamService.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.Exam
{
    [ApiController]
    [Route("v1/api/exams")]
    public class ExamController : Controller
    {
        private readonly IExamPaperService examService;
        private readonly int pageSize = 3;
        public ExamController(IExamPaperService examService)
        {
            this.examService = examService; 
        }
        [HttpGet("{id}")]
        public async Task<ObjectResult> Get([FromRoute]int id)
        {
            var response  = await examService.GetExam(id);
            return response;
        }

        [HttpGet("all")]
        public async Task<ObjectResult> GetAll([FromQuery][Required] int pageIndex)
        {
            PagingRequest pagingRequest = new PagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var response = await examService.GetAllExams(x => true, pagingRequest);
            return response;
        }
        [HttpGet("leader/{leaderId}")]
        public async Task<ObjectResult> GetByLeaderId([FromQuery][Required] int pageIndex, [FromRoute] int leaderId)
        {
            PagingRequest pagingRequest = new PagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var response = await examService.GetAllExams(x => x.ExamSchedule.LeaderId == leaderId, pagingRequest);
            return response;
        }
        [HttpPost("")]
        public async Task<ObjectResult> Create([FromBody] ExamCreateRequestModel examCreateRequestModel)
        {
            var response = await examService.CreateExam(examCreateRequestModel);
            return response;
        }
        [HttpPut("{id}")]
        public async Task<ObjectResult> Update([FromRoute] int id, [FromBody] ExamUpdateRequestModel examUpdateRequestModel)
        {
            var response = await examService.UpdateExam(id, examUpdateRequestModel);
            return response;
        }
        [HttpDelete("{id}")]
        public async Task<ObjectResult> Delete([FromRoute] int id)
        {
            return await examService.DeleteExam(id);
        }
        [HttpPut("review-exam")]
        public async Task<ObjectResult> ReviewExam([FromBody] ReviewExamModel reviewExamModel)
        {
            var response = await examService.ApproveExam(reviewExamModel.CommentModel, reviewExamModel.ExamUpdateApproveModel);
            return response;
        }
    }
}
