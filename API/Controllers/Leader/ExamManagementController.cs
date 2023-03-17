using Business.ExamSchedule.interfaces;
using Business.ExamSchedule.Models;
using Business.UserService.Interfaces;
using Business.UserService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.Leader
{
    [ApiController]
    public class ExamManagementController : ControllerBase
    {
        private readonly IExamScheduleService _examManagementService;

        public ExamManagementController(IExamScheduleService examManagementService)
        {
            _examManagementService = examManagementService;
        }

        [HttpPost]
        [Route("api/leader/exams-schedule")]
        public async Task<IActionResult> CreateExamSchedule(CreateExamScheduleModel createExamScheduleModel,[Required] int availableId)
        {
            var response = await _examManagementService.CreateExamSchedule(createExamScheduleModel, availableId);
            return Ok(response);
        }

        [HttpGet]
        [Route("api/leader/examSchedule/getAllExamSchedule")]
        public async Task<IActionResult> GetAllExamScheduleByLeaderId(int leaderId)
        {
            var response = await _examManagementService.GetAllExamScheduleByLeaderId(leaderId);
            if(response.StatusCode == 404)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("api/leader/examSchedule/getExamSchedule/{id}")]
        public async Task<IActionResult> GetExamSchedule(int id)
        {
            var response = await _examManagementService.GetExamSchedule(id);
            if(response.StatusCode == 404)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
