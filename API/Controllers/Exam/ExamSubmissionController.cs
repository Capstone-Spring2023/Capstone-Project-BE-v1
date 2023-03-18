﻿using Business;
using Business.ExamPaperService.Interfaces;
using Data.Paging;
using Microsoft.AspNetCore.Mvc;
using Business.ExamService.Models;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Exam
{
    [ApiController]
    [Route("api/exam-submission")]
    public class ExamSubmisionController : Controller
    {
        private readonly IExamPaperService examService;
        private readonly int pageSize = 3;
        public ExamSubmisionController(IExamPaperService examService)
        {
            this.examService = examService; 
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Exam Submission by it's Id")]
        public async Task<ObjectResult> Get([FromRoute]int id)
        {
            var response  = await examService.GetExam(id);
            return response;
        }

        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get Exam-Submissions and paging")]
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
        [HttpGet("api/user/{userId}/exam-submission")]
        [SwaggerOperation(Summary = "Get Exam-Submission by UserId")]
        public async Task<ObjectResult> GetByLeaderId([FromRoute] int userId)
        {
            PagingRequest pagingRequest = new PagingRequest()
            {
                PageIndex = 1,
                PageSize = 20
            };
            var response = await examService.GetAllExams(x => x.ExamSchedule.RegisterSubject.UserId == userId, pagingRequest);
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

    }
}
