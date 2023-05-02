using Business.ExamSchedule.Models;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.Statistic
{
    public class StatisticResponse
    {
        public int UserId { get; set; }
        public string fullName { get; set; }
        public int NumNotRegisteredSubject { get; set; }
        public int? NumClass { get; set; }
        public double? UPoint { get; set; }
        public int satisfyPoint { get; set; }
        public int SemesterId { get; set; }
        public double? AlphaIndex { get; set; }
        public double? PercentPoint { get; set; }
        public double percentAlphaIndex { get; set; }   
    }
    
    [Route("api/statistic")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly CFManagementContext _context;
        public StatisticController(CFManagementContext context)
        {
            _context = context;
        }
        [HttpGet("all/semester/{semesterId}")]
        public async Task<ObjectResult> GetStatistics([FromRoute]int semesterId)
        {
            var res = _context.PointIndices
                .Include(x=> x.User)
                .Where( x=> x.SemesterId == semesterId & x.UserId != -1)
                .ToList();
            var a = _context.PointIndices.First(x=> x.SemesterId == semesterId && x.UserId == -1);

            var res1 = res.Select(x => new StatisticResponse()
            {
                fullName = x.User.FullName,
                UPoint = x.UPoint,
                NumClass = x.NumClass,
                AlphaIndex = x.AlphaIndex,
                SemesterId = x.SemesterId,
                UserId = x.UserId,
                PercentPoint = ((double)x.UPoint / (double)a.UPoint) * 100,
                percentAlphaIndex = ((double)x.AlphaIndex / (double)a.AlphaIndex) * 100,
                satisfyPoint = (int)((double)x.UPoint / (double)x.AlphaIndex)
            }).ToList();
            foreach(var item in res1)
            {
                var numNotRegistered = _context.RegisterSubjects
                    .Where(x => x.UserId == item.UserId && x.IsRegistered == false && x.Status == true  && x.AvailableSubject.SemesterId == 1 )
                    .Count();
                item.NumNotRegisteredSubject = numNotRegistered;
            }
            return new ObjectResult(res1)
            {
                StatusCode = 200,
            };
        }
        [HttpGet("user/{userId}/semester/{semesterId}")]
        public async Task<ObjectResult> GetStatistic([FromRoute] int userId, [FromRoute]int semesterId)
        {
            var x = _context.PointIndices.FirstOrDefault(x=> x.UserId == userId && x.SemesterId == semesterId);
            var a = _context.PointIndices.First(x => x.SemesterId == semesterId && x.UserId == -1);
            var res = new StatisticResponse()
            {
                UserId = userId,
                SemesterId = semesterId,
                AlphaIndex = x.AlphaIndex,
                NumClass = x.NumClass,
                UPoint = x.UPoint,
                PercentPoint = ((double)x.UPoint / (double)a.UPoint) * 100,
                percentAlphaIndex = ((double)x.AlphaIndex / (double)a.AlphaIndex) * 100,
                satisfyPoint = (int)((double)x.UPoint / (double)x.AlphaIndex)
            };
            if (res== null)
            {
                return new ObjectResult("Not Found")
                {
                    StatusCode = 400,
                };
            }
            else
            {
                return new ObjectResult(res)
                {
                    StatusCode = 200,
                };
            }
        }
        [HttpGet("Semester/getAll")]
        public async Task<ObjectResult> GetAllSemester()
        {
            var listSemester = await _context.Semesters.ToListAsync();
            return new ObjectResult(listSemester)
            {
                StatusCode = 200
            };
        }

        [HttpGet("StatisticalForLecturerOrLeader/{currentUserId}")]
        public async Task<ObjectResult> StatisticalForLecturerOrLeader(int currentUserId, int semesterId)
        {
            if (_context.Users.Find(currentUserId).RoleId != 1)
            {
                //tổng số môn cần phải duyệt
                var response = new StatisticalModelForLecturerOrLeader();
                var examSchedulesOfApprovalUser = await _context.ExamSchedules
                    .Where(x => x.AppovalUserId == currentUserId && x.Status && x.RegisterSubject.AvailableSubject.SemesterId == semesterId)
                    .ToListAsync();
                var listExamPaper = new List<ExamPaper>();
                foreach (var examSchedule in examSchedulesOfApprovalUser)
                {
                    var examPaper = _context.ExamPapers
                        .Where(x => x.ExamScheduleId == examSchedule.ExamScheduleId && x.Status == ExamPaperStatus.APPROVED)
                        .FirstOrDefault();
                    if (examPaper != null)
                    {
                        listExamPaper.Add(examPaper);
                    }
                }
                var totalExamNeedApprove = examSchedulesOfApprovalUser.Count() - listExamPaper.Count();
                response.totalExamNeedApprove = totalExamNeedApprove;
                // tổng số môn cần phải nộp
                listExamPaper.Clear();
                var examSchedulesOfUser = await _context.ExamSchedules
                    .Where(x => x.RegisterSubject.UserId == currentUserId && x.Status && x.RegisterSubject.AvailableSubject.SemesterId == semesterId)
                    .ToListAsync();
                foreach (var examSchedule in examSchedulesOfUser)
                {
                    var examPaper = _context.ExamPapers
                        .Where(x => x.ExamScheduleId == examSchedule.ExamScheduleId && x.Status == ExamPaperStatus.APPROVED)
                        .FirstOrDefault();
                    if (examPaper != null)
                    {
                        listExamPaper.Add(examPaper);
                    }
                }
                var totalExamNeedSubmit = examSchedulesOfUser.Count() - listExamPaper.Count();
                response.totalExamNeedSubmit = totalExamNeedSubmit;
                // tổng số lớp học trong kì
                var registerSubject = await _context.RegisterSubjects
                    .Where(x => x.UserId == currentUserId && x.Status && x.AvailableSubject.SemesterId == semesterId)
                    .ToListAsync();
                var totalClassTeaching = registerSubject.Count();
                response.totalClassTeaching = totalClassTeaching;
                return new ObjectResult(response)
                {
                    StatusCode = 200
                };
            } else
            {
                var response = new StatisticalModelForHeader();
                //tổng số đề cần nộp của header
                var listExamPaper = new List<ExamPaper>();
                var examSchedulesOfUser = await _context.ExamSchedules
                    .Where(x => x.RegisterSubject.UserId == currentUserId && x.Status && x.RegisterSubject.AvailableSubject.SemesterId == semesterId)
                    .ToListAsync();
                foreach (var examSchedule in examSchedulesOfUser)
                {
                    var examPaper = _context.ExamPapers
                        .Where(x => x.ExamScheduleId == examSchedule.ExamScheduleId && x.Status == ExamPaperStatus.APPROVED)
                        .FirstOrDefault();
                    if (examPaper != null)
                    {
                        listExamPaper.Add(examPaper);
                    }
                }
                var totalExamNeedSubmitOfHeader = examSchedulesOfUser.Count() - listExamPaper.Count();
                response.totalExamNeedSubmittedOfHeader = totalExamNeedSubmitOfHeader;
                //tổng số đề phải nộp của gv              
                var examSchedules = await _context.ExamSchedules
                    .Where(x => x.Status && x.RegisterSubject.AvailableSubject.SemesterId == semesterId)
                    .ToListAsync();
                var totalExamNeedSubmitOfAllTeacher = examSchedules.Count();
                response.totalExamNeedSubmittedOfTeacher = totalExamNeedSubmitOfAllTeacher;
                //tổng số đề đã nộp của gv
                listExamPaper.Clear();
                foreach (var examSchedule in examSchedules)
                {
                    var examPaper = _context.ExamPapers
                        .Where(x => x.ExamScheduleId == examSchedule.ExamScheduleId && x.Status == ExamPaperStatus.APPROVED)
                        .FirstOrDefault();
                    if (examPaper != null)
                    {
                        listExamPaper.Add(examPaper);
                    }
                }
                //tổng số đề còn thiếu của gv
                response.totalExamNotSubmitOfTeacher = response.totalExamNeedSubmittedOfTeacher - response.totalExamSubmittedOfTeacher;
                return new ObjectResult(response)
                {
                    StatusCode = 200
                };
            }
        }
        
    }
}
