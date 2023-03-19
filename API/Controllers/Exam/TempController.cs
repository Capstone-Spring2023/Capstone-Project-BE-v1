using AutoMapper;
using Business.AvailableSubjectService.Models;
using Business.Constants;
using Business.ExamSchedule.Models;
using Business.ExamService.Models;
using Data.Models;
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
                .Where(x => x.RegisterSubject.UserId == userId)
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
                data.SubjectName = _context.Subjects.FirstOrDefault(x => x.SubjectId == examSchedule.SubjectId).SubjectName;

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
                .Where(x => x.RegisterSubject.UserId == userId)
                .ToListAsync();
            if (examSchedules == null || examSchedules.Count() == 0)
            {
                return new ObjectResult(new List<Object>())
                {
                    StatusCode = 404,
                };
            }
            var res = new List<AvailableSubjectResponse>(); 
            foreach (var examSchedule in examSchedules)
            {
                var register = await _context.RegisterSubjects.FirstOrDefaultAsync(x => x.RegisterSubjectId == examSchedule.RegisterSubjectId);
                var availableSubject = await _context.AvailableSubjects.FirstOrDefaultAsync(x => x.AvailableSubjectId == register.AvailableSubjectId);
                res.Add(_mapper.Map<AvailableSubjectResponse>(availableSubject));
            }
            return new ObjectResult(res)
            {
                StatusCode = 200,
            };
        }
    }
}
