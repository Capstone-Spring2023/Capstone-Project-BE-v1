using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.Statistic
{
    
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
                .Where( x=> x.SemesterId == semesterId & x.UserId != -1)
                .ToList();
            var a = _context.PointIndices.First(x=> x.SemesterId == semesterId && x.UserId == -1);
            foreach (var item in res)
            {
                item.PercentPoint = (double)a.UPoint / (double)item.UPoint;
            }
            return new ObjectResult(res)
            {
                StatusCode = 200,
            };
        }
        [HttpGet("user/{userId}/semester/{semesterId}")]
        public async Task<ObjectResult> GetStatistic([FromRoute] int userId, [FromRoute]int semesterId)
        {
            var res = _context.PointIndices.FirstOrDefault(x=> x.UserId == userId && x.SemesterId == semesterId);
            var a = _context.PointIndices.First(x => x.SemesterId == semesterId && x.UserId == -1);
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
