using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using API.Controllers.Schedules.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Schedules
{
    public class UserAlphaResponse
    {
        public int UserId { get; set; } 
        public double alphaIndex { get; set; }   
        public int numMinClass { get; set; } = 0;
        public double percent { get; set; }
    }
    [Route("api/schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly CFManagementContext _context;
        public ScheduleController(CFManagementContext context)
        {
            _context = context;
        }
        [HttpGet("alpha-index-and-min-class")]
        public async Task<ObjectResult> getAlpha()
        {
            var users = await _context.Users.Skip(1).ToListAsync();
            var sum = users.Sum(x => x.AlphaIndex);
            var res = users.Select(x => new UserAlphaResponse()
            {
                UserId = x.UserId,
                alphaIndex =(double) x.AlphaIndex,
                numMinClass = (int) x.NumMinClass,
                percent = ((double)x.AlphaIndex / (double)sum) *100
            });
            return new ObjectResult(res);
        }
        [HttpPost("register-subject-slot")]
        public async Task<ObjectResult> register(RegisterSubjectSlot registerSubjectSlot)
        {
            var list = new List<RegisterSubject>();
            foreach (var i in registerSubjectSlot.availableSubjectIds)
            {
                RegisterSubject registerSubject = new RegisterSubject()
                {
                    UserId = registerSubjectSlot.userId,
                    AvailableSubjectId = i,
                    ClassId = 123,
                    RegisterDate = DateTime.Now,
                    Status = true,
                    IsRegistered = true,
                };
                list.Add(registerSubject);
            }
            _context.AddRange(list);
            await _context.SaveChangesAsync();
            var list1 = new List<RegisterSlot>();
            foreach(var a in registerSubjectSlot.registerSlots)
            {
                RegisterSlot registerSlot = new RegisterSlot()
                {
                    SemesterId = 1,
                    Slot = a,
                    Status = true,
                    UserId = registerSubjectSlot.userId
                };
                list1.Add(registerSlot);
            }
            _context.AddRange(list1);
            await _context.SaveChangesAsync();
            return new ObjectResult("Create Success")
            {
                StatusCode = 200,
            };
        }
        [HttpGet("lecturer/{lecturerId}")]
        public async Task<ObjectResult> getScheduleByLecturer([FromRoute] int lecturerId)
        {
            var a = _context.Schedules
                .Include(x => x.Class)
                .Where(x => x.Class.RegisterSubject.UserId == lecturerId)
                .Select(x => new ScheduleResponse()
                    {
                        ClassId = x.ClassId,
                        ScheduleDate = x.ScheduleDate,
                        ClassCode = x.Class.ClassCode,
                        ScheduleId = x.ScheduleId,
                        Slot = x.Slot,
                    })
                .ToList();
            return new ObjectResult(a)
            {
                StatusCode = 200
            };
        }
        [HttpPut("lecturer")]
        public async Task<ObjectResult> updateSchedule([FromBody] ScheduleUpdateRequest request)
        {
            
            var a = _context.Classes
                .Include(x => x.RegisterSubject)
                .First(x=> x.ClassId == request.ClassId);
            var registerSubjectId = _context.RegisterSubjects
                .FirstOrDefault(
                x => x.UserId == request.UserId
                && x.AvailableSubjectId == a.RegisterSubject.AvailableSubjectId).RegisterSubjectId;

            a.RegisterSubjectId = registerSubjectId;
            await _context.SaveChangesAsync();
            return new ObjectResult("OK")
            {
                StatusCode = 200,
            };
        }
    }
}
