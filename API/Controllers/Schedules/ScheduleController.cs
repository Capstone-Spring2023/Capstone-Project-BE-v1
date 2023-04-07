using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using API.Controllers.Schedules.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Schedules
{
    [Route("api/schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly CFManagementContext _context;
        public ScheduleController(CFManagementContext context)
        {
            _context = context;
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
        [HttpGet("lecturer/{lecturerId}/schedule/")]
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
        private readonly string UPDATE_ONE_SCHEDULE = "one";
        private readonly string UPDATE_ALL_SCHEDULE = "all";
        [HttpPut("lecturer/schedule")]
        public async Task<ObjectResult> updateSchedule([FromBody] ScheduleUpdateRequest request)
        {
            if (request.Type == UPDATE_ONE_SCHEDULE)
            {
                Schedule schedule = new Schedule()
                {
                    ScheduleDate = request.ScheduleDate,
                    ScheduleId = request.ScheduleId,
                    Slot = request.Slot,
                    ClassId = request.ClassId,
                };
                var track = _context.Attach(schedule);
                track.State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new ObjectResult("OK")
                {
                    StatusCode = 200,
                };
            }
            else if (request.Type == UPDATE_ALL_SCHEDULE)
            {
                var schedules = _context.Schedules
                    .Where(x => x.ClassId == request.ClassId && x.ScheduleDate.DayOfWeek == request.ScheduleDate.DayOfWeek);
                foreach (var a in schedules)
                {

                    a.Slot = request.Slot;
                }
                await _context.SaveChangesAsync();
                return new ObjectResult("OK")
                {
                    StatusCode = 200,
                };
            }
            else return new ObjectResult("Wrong type")
            {
                StatusCode = 400,
            };
        }
    }
}
