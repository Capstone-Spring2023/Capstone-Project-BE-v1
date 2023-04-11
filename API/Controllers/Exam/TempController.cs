using AutoMapper;
using Business.AvailableSubjectService.Models;
using Business.Constants;
using Business.ExamSchedule.Models;
using Business.ExamService.Models;
using Business.RegisterSubjectService.Models;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public TempController(CFManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("user/{userId}/exam-schedule")]
        public async Task<ObjectResult> getExamScheduleByUserId([FromRoute] int userId)
        {
            var examSchedules = await _context.ExamSchedules
                .Where(x => x.RegisterSubject.UserId == userId && x.Status)
                .ToListAsync();
            var examScheduleResponses = new List<ResponseExamSchedule>();
            foreach (var examSchedule in examSchedules)
            {
                var register = await _context.RegisterSubjects
                    .FirstOrDefaultAsync(x => x.RegisterSubjectId == examSchedule.RegisterSubjectId);
                var availableSubject = await _context.AvailableSubjects
                    .FirstOrDefaultAsync(x => x.AvailableSubjectId == register.AvailableSubjectId);
                // Map
                var a = _mapper.Map<ResponseExamSchedule>(examSchedule);
                a.LeaderName = availableSubject.LeaderName;
                a.SubjectName = availableSubject.SubjectName;
                examScheduleResponses.Add(a);
            }
            if (examScheduleResponses == null || examScheduleResponses.Count() == 0)
            {
                return new ObjectResult(new List<object>())
                {
                    StatusCode = 404,
                };
            }
            return new ObjectResult(examScheduleResponses)
            {
                StatusCode = 200
            };
        }
        [HttpGet("leader/{leaderId}/exam-submission")]
        public async Task<ObjectResult> getExamPaperByLeaderId([FromRoute] int leaderId)
        {
            var ExamPapers = await _context.ExamPapers
                .Where(x => x.Status == ExamPaperStatus.PENDING && x.ExamSchedule.LeaderId == leaderId)
                .ToListAsync();
            List<ExamResponseModel> datas = ExamPapers.Select(x => _mapper.Map<ExamResponseModel>(x)).ToList();
            foreach (var data in datas)
            {
                var examSchedule = _context.ExamSchedules.FirstOrDefault(x => x.ExamScheduleId == data.ExamScheduleId);
                var ASubject = _context.AvailableSubjects.FirstOrDefault(x => x.AvailableSubjectId == examSchedule.AvailableSubjectId);
                data.SubjectName = ASubject.SubjectName;
                data.Tittle = examSchedule.Tittle;

                int typeId = _context.Subjects.First(x => x.SubjectId == ASubject.SubjectId).TypeId;
                data.Type = _context.Types.First(x => x.TypeId == typeId).TypeName;
                var register = _context.RegisterSubjects.Find(examSchedule.RegisterSubjectId);
                data.LecturerName = _context.Users.Find(register.UserId).FullName;

                var comment = ExamPapers.FirstOrDefault(x => x.ExamPaperId == data.ExamPaperId).Comments.FirstOrDefault();
                if (comment == null)
                {
                    data.Comment = "";
                }
                else
                {
                    data.Comment = comment.CommentContent.Trim();
                }
            }
            return new ObjectResult(datas)
            {
                StatusCode = 200,
            };
        }
        [HttpGet("user/{userId}/exam-schedule/available-subject")]
        [SwaggerOperation(Summary = "API lấy ra danh sách môn mà user đó có request")]
        public async Task<ObjectResult> getAvailableSubjectWithExamScheduleByUserId([FromRoute] int userId)
        {
            var examSchedules = await _context.ExamSchedules
                .Where(x => x.RegisterSubject.UserId == userId && x.Status)
                .ToListAsync();
            if (examSchedules == null || examSchedules.Count() == 0)
            {
                return new ObjectResult(new List<Object>())
                {
                    StatusCode = 404
                };
            }
            var res = new List<AvailableSubjectResponse>();
            foreach (var examSchedule in examSchedules)
            {
                var examPaper = await _context.ExamPapers.Where(x => x.ExamScheduleId == examSchedule.ExamScheduleId && x.Status != "Rejected").FirstOrDefaultAsync();
                if (examPaper != null)
                {
                    continue;
                }
                var register = await _context.RegisterSubjects.FirstOrDefaultAsync(x => x.RegisterSubjectId == examSchedule.RegisterSubjectId);
                var availableSubject = await _context.AvailableSubjects.FirstOrDefaultAsync(x => x.AvailableSubjectId == register.AvailableSubjectId);
                var type = await _context.Types.FirstOrDefaultAsync(x => x.TypeId == examSchedule.TypeId);
                var availableSubjectresponse = _mapper.Map<AvailableSubjectResponse>(availableSubject);
                availableSubjectresponse.ExamScheduleId = examSchedule.ExamScheduleId;
                availableSubjectresponse.TypeName = type.TypeName;
                res.Add(availableSubjectresponse);

            }
            return new ObjectResult(res)
            {
                StatusCode = 200,
            };
        }
        [HttpGet("user/{userId}/register-subject-slot")]
        [SwaggerOperation(Summary = "API lấy ra danh sách register subject + slot của 1 user")]
        public async Task<ObjectResult> getRegisterSubjects([FromRoute] int userId)
        {
            var registerSubjects = await _context.RegisterSubjects
                .Where(x => x.UserId == userId)
                .ToListAsync();
            var registerSubjectResponse = new RegisterSubjectResponse();
            registerSubjectResponse.RegisterDate = registerSubjects[0].RegisterDate;
            registerSubjectResponse.Status = registerSubjects[0].Status;
            foreach(var register in registerSubjects)
            {
                var avaibaleSubject = _context.AvailableSubjects.Find(register.AvailableSubjectId);
                if (avaibaleSubject.Status)
                {
                    var subjectName = _context.Subjects.Find(avaibaleSubject.SubjectId).SubjectName;
                    registerSubjectResponse.SubjectName.Add(subjectName);
                }
                
            }
            var registerSlots = _context.RegisterSlots.Where(x => x.UserId == userId)
                .Select(x => _mapper.Map<RegisterSlotResponse>(x)).ToList();

            var res = new RegisterSubjectSlotResponse()
            {
                registerSlots = registerSlots.Select(x=> x.Slot.Trim()).ToList(),
                registerSubjects = registerSubjectResponse
            }
            ;

            return new ObjectResult(res)
            {
                StatusCode = 200
            };
         
        }
    }
}
