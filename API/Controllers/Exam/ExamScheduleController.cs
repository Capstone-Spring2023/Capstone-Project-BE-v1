using Business.AvailableSubjectService.Interface;
using Business.ExamSchedule.interfaces;
using Business.ExamSchedule.Models;
using Business.NotificationService.Model;
using Data.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.Exam
{
    [Route("")]
    [ApiController]
    public class ExamScheduleController : ControllerBase
    {
        private readonly IExamScheduleService _examManagementService;
        private readonly IAvailableSubjectService _avaibleSubjectService;
        private readonly int pageSize = 10;
        public ExamScheduleController(IExamScheduleService examManagementService, IAvailableSubjectService availableSubjectService)
        {
            _examManagementService = examManagementService;
            _avaibleSubjectService = availableSubjectService;
        }

        /*[HttpGet("api/available-subject")]
        [SwaggerOperation(Summary = "")]
        public async Task<ObjectResult> GetAll([FromQuery] int pageIndex)
        {
            PagingRequest pagingRequest = new PagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var response = await _avaibleSubjectService.GetAvailableSubjects(x => true, pagingRequest);
            return response;
        }*/

        [HttpGet]
        [Route("api/leader/{leaderId}/available-subject")]
        [SwaggerOperation(Summary = "Get Available Subjects belong to a leader (and haven't have exam schedule yet)")]
        public async Task<IActionResult> getAllAvailableSubjectByLeaderId([FromRoute] int leaderId)
        {
            var response = await _avaibleSubjectService.GetAllAvailableSubjectByLeaderId(leaderId);
            if (response.StatusCode == (int)Business.Constants.StatusCode.NOTFOUND)
            {
                return NotFound(new List<Object>());
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("api/exam-schedule/{id}")]
        [SwaggerOperation(Summary = "Get Exam-Schedule By it's ID")]
        public async Task<IActionResult> GetExamSchedule([FromRoute] int id)
        {
            var response = await _examManagementService.GetExamSchedule(id);
            if (response.StatusCode == 404)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("api/leader/{leaderId}/exam-schedule")]
        [SwaggerOperation(Summary = "Get Exam-Schedules By LeaderId")]
        public async Task<IActionResult> GetAllExamScheduleByLeaderId([FromRoute] int leaderId)
        {
            var response = await _examManagementService.GetAllExamScheduleByLeaderId(leaderId);
            if (response.StatusCode == 404)
            {
                return NotFound(new List<Object>());
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("api/exam-schedule/available-subject/{availableSubjectId}")]
        [SwaggerOperation(Summary = "Get Detail Request ExamSchedule By availableSubjectId")]
        public async Task<IActionResult> GetDetailRequestExamSchedule(int availableSubjectId)
        {
            var response = await _examManagementService.GetDetailRequestExamSchedule(availableSubjectId);
            if(response.StatusCode == 404)
            {
                return NotFound(new List<Object>());
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("api/exam-schedule")]
        [SwaggerOperation(Summary = "Create Exam-Schedule")]
        public async Task<IActionResult> CreateExamSchedule(CreateExamScheduleModel createExamScheduleModel, [Required] int availableId)
        {
            var response = await _examManagementService.CreateExamSchedule(createExamScheduleModel, availableId);
            return Ok(response);
        }
        [HttpPut]
        [Route("api/exam-schedule/{availableSubjectId}")]
        [SwaggerOperation(Summary = "Update all Exam-Schedules with A-SubjectId by 3 field: title, deadline and ExamLink")]
        public async Task<ObjectResult> Update(UpdateExamScheduleModel updateExamScheduleModel,[FromRoute] int availableSubjectId)
        {
            var response = await _examManagementService.UpdateExamSchedule(updateExamScheduleModel, availableSubjectId);
            return response;
        }
        [HttpDelete]
        [Route("api/exam-schedule/{availableSubjectId}")]
        [SwaggerOperation(Summary = "Delete Exam-Schedule by it's ID")]
        public async Task<ObjectResult> Delete([FromRoute] int availableSubjectId)
        {
            return await _examManagementService.DeleteExamSchedule(availableSubjectId);
        }

        [HttpGet("api/teachers/subject/{subjectId}")]
        public async Task<ObjectResult> GetTeachers([FromRoute] int subjectId, [FromQuery] int pageIndex)
        {
            PagingRequest pagingRequest = new PagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            return await _avaibleSubjectService.GetTeachersBySubjectId(subjectId, pagingRequest);
        }

    }
}
