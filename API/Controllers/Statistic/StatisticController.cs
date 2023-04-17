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
                UserId  = x.UserId,
                PercentPoint = ((double)x.UPoint/ (double)a.UPoint ) * 100,
                percentAlphaIndex = ((double)x.AlphaIndex/(double)a.AlphaIndex) * 100
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
                percentAlphaIndex = ((double)x.AlphaIndex / (double)a.AlphaIndex) * 100
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
        
    }
}
