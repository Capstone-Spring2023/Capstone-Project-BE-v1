using AutoMapper;
using Business.AvailableSubjectService.Interface;
using Business.AvailableSubjectService.Models;
using Business.Constants;
using Business.ExamPaperService.Interfaces;
using Business.ExamSchedule.interfaces;
using Business.ExamSchedule.Models;
using Business.ExamService.Models;
using Business.RegisterSubjectService.Interfaces;
using Business.RegisterSubjectService.Models;
using Data;
using Data.Models;
using Data.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Exam
{
    [Route("api")]
    [ApiController]
    public class TempController : ControllerBase
    {
        private readonly CFManagementContext _context;
        private readonly IMapper _mapper;
        private readonly IExamScheduleService _examScheduleService;
        private readonly IExamPaperService _examPaperService;
        private readonly IAvailableSubjectService _availableSubjectService;
        private readonly IRegisterSubjectService _registerSubjectService;
        public TempController(CFManagementContext context, IMapper mapper, IExamPaperService examPaperService,
            IExamScheduleService examScheduleService,IAvailableSubjectService availableSubjectService, IRegisterSubjectService registerSubjectService)
        {
            _examPaperService = examPaperService;
            _examScheduleService = examScheduleService;
            _availableSubjectService = availableSubjectService;
            _registerSubjectService = registerSubjectService;
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("user/{userId}/exam-schedule")]
        public async Task<ObjectResult> getExamScheduleByUserId([FromRoute] int userId)
        {
            var res = await _examScheduleService.GetExamSchedulesByUserId(userId);
            return res;
        }
        [HttpGet("approval-user/{currentUserId}/exam-submission")]
        [SwaggerOperation(Summary = "API lấy ra danh sách Pending của Approval User - main")]
        public async Task<ObjectResult> getExamPaperByLeaderId([FromRoute] int currentUserId)
        {
            var ExamPapers = await _examPaperService.getExamPaperPendingByAppovalUserId(currentUserId);
            return ExamPapers;
        }

        [HttpGet("user/{userId}/exam-schedule/available-subject")]
        [SwaggerOperation(Summary = "API lấy ra danh sách môn mà user đó có request")]
        public async Task<ObjectResult> getAvailableSubjectWithExamScheduleByUserId([FromRoute] int userId)
        {
            var res = await _availableSubjectService.getAvailableSubjectWithExamScheduleByUserId(userId);
            return res;
        }
        [HttpGet("user/{userId}/register-subject-slot")]
        [SwaggerOperation(Summary = "API lấy ra danh sách register subject + slot của 1 user")]
        public async Task<ObjectResult> getRegisterSubjects([FromRoute] int userId)
        {
            var res =await _registerSubjectService.getRegisterSubjects(userId);
            return res;
        }

        [HttpGet("approval-user/{appovalUserId}/exam-submission-approved")]
        [SwaggerOperation(Summary = "API lấy ra danh sách approved của Approval User")]
        public async Task<ObjectResult> getExamPaperApprovedByAppovalUserId([FromRoute] int appovalUserId)
        {
            var res = await _examPaperService.getExamPaperApprovedByApprovalUserId(appovalUserId);
            return res;
        }
    }
}
