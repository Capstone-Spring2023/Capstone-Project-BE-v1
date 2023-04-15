using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using API.Controllers.Schedules.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using AutoScheduling.Reader;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Schedules
{
    public class UpdateScheduleRequest
    {
        [Required]
        public int oldUserId { get; set; }
        [Required]
        public int newUserId { get; set; }
        [Required]
        public int classId { get; set; }
        [Required]
        public int semesterId { get; set; }
    }
    public class CheckingUpdateResponse
    {
        [Required]
        public double oldUserPercentAfterChange { get; set; }
        [Required]
        public double oldUserPointAfterChange { get; set; }
        [Required]
        public double newUserPercentAfterChange { get; set; }
        [Required]
        public double newUserPointAfterChange { get; set; }
        [Required]
        public double average { get; set; }
    }
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
        [HttpPost("/update-checking")]
        public async Task<ObjectResult> UpdateSchedule(UpdateScheduleRequest request)
        {


            Class @class = _context.Classes.First(x => x.ClassId == request.classId);
            
            var oldUserRegisterSlots = _context.RegisterSlots.Where(x => x.UserId == request.oldUserId && x.SemesterId == request.semesterId)
                .ToList();
            var oldUserRegisterSubjects = _context.RegisterSubjects
                .Include(x => x.AvailableSubject)
                .Where(x => x.UserId == request.oldUserId && x.AvailableSubject.SemesterId == request.semesterId && x.IsRegistered == true)
                .ToList();
            var oldUserClass = _context.Classes
                .Where(x => x.RegisterSubject.UserId == request.oldUserId && x.ClassAsubjects.First().Asubject.SemesterId == request.semesterId)
                .ToList();
            var oldDNum = _context.Users.First(x => x.UserId == request.oldUserId).NumMinClass;
            oldUserClass.Remove(oldUserClass.First(x=> x.ClassId == request.classId));

            var oldUserPointIndex = _context.PointIndices.First(x=> x.UserId == request.oldUserId && x.SemesterId == request.semesterId);

            var newUserRegisterSlots = _context.RegisterSlots.Where(x => x.UserId == request.newUserId && x.SemesterId == request.semesterId)
                .ToList();
            
            var newUserRegisterSubjects = _context.RegisterSubjects
                .Include(x=> x.AvailableSubject)
                .Where(x => x.UserId == request.newUserId && x.AvailableSubject.SemesterId == request.semesterId && x.IsRegistered ==true)
                .ToList();
            var newUserClass = _context.Classes
                .Where(x => x.RegisterSubject.UserId == request.newUserId && x.ClassAsubjects.First().Asubject.SemesterId == request.semesterId)
                .ToList();
            if (newUserClass.Exists(x => x.Slot == @class.Slot))
            {
                return new ObjectResult("Duplicate slot in new user")
                {
                    StatusCode = 400
                };
            }

            var newDNum = _context.Users.First(x => x.UserId == request.newUserId).NumMinClass;
            newUserClass.Add(@class);

            var newUserPointIndex = _context.PointIndices.First(x => x.UserId == request.newUserId && x.SemesterId == request.semesterId);

            
            var sumPoint = _context.PointIndices.First(x=> x.UserId == -1 && x.SemesterId == request.semesterId);
            sumPoint.UPoint -= (oldUserPointIndex.UPoint + newUserPointIndex.UPoint);

            var oldUserUPoint_AfterChange = calculateUi(request.oldUserId, oldUserRegisterSlots, oldUserRegisterSubjects, oldUserClass, oldUserPointIndex,(int)oldDNum);
            var newUserUPoint_AfterChange = calculateUi(request.newUserId, newUserRegisterSlots, newUserRegisterSubjects, newUserClass, newUserPointIndex,(int)newDNum);
            
            sumPoint.UPoint += (oldUserUPoint_AfterChange + newUserUPoint_AfterChange);



            CheckingUpdateResponse response = new CheckingUpdateResponse()
            {

                oldUserPercentAfterChange = oldUserUPoint_AfterChange / (double)sumPoint.UPoint * 100,
                oldUserPointAfterChange = oldUserUPoint_AfterChange,
                newUserPointAfterChange = newUserUPoint_AfterChange,
                newUserPercentAfterChange = newUserUPoint_AfterChange / (double)sumPoint.UPoint * 100,
                average = 32 / 100

            };
            return new ObjectResult(response)
            {
                StatusCode = 200,
            };
        }

        private  double calculateUi(int userId,List<RegisterSlot> registerSlots, List<RegisterSubject> registerSubjects, List<Class> classes, PointIndex pointIndex, int d)
        {
            //var oldU = pointIndex.UPoint / pointIndex.AlphaIndex;
            List<(string, string)> list = new List<(string, string)>()
            {
                ("A1","A2"),("P1","P2"),
                ("A3","A4"),("P3","P4"),
                ("A5","A6"),("P5","P6")
            };
            double u = 20;
            foreach(var a in classes)
            {
                //Check register Subject
                var subjectName = a.ClassCode.Split('_')[0];
                if (! registerSubjects.Exists(x=> x.AvailableSubject.SubjectName == subjectName))
                {
                    
                    u--;
                    Console.WriteLine($"Minus on subject {subjectName} - u: {u}");
                }
                // Check register slots
                var slot = list.First(x=> x.Item1 == a.Slot || x.Item2 == a.Slot).Item1;
                if (!registerSlots.Exists(x=> x.Slot == slot))
                {
                    u--;
                    Console.WriteLine($"Minus on subject {slot} - u: {u}");
                }
            }
            var day_slots = new List<(int, int)>();
            foreach (var a in classes)
            {
                int day, slot;
                ClassDaySlotReader.APx_to_day_slot(a.Slot, out day, out slot);
                day_slots.Add((day, slot));
            }
            day_slots = day_slots.OrderBy(x => (x.Item1, x.Item2)).ToList();
            int previous_day = -1, previous_slot = -1;
            //check độ khít
            foreach (var day_slot in day_slots)
            {
                if (previous_day != -1 && previous_day == day_slot.Item1)
                {
                    u = u - (day_slot.Item2 - previous_slot) + 1;
                    Console.WriteLine($"Minus on day_slot- day: {day_slot.Item1} - slot: {day_slot.Item2} - u: {u}");
                }
                previous_day = day_slot.Item1;
                previous_slot = day_slot.Item2;
            }

            int count_num_teaching_class = classes.Count();
            if (count_num_teaching_class < d || count_num_teaching_class > 10)
            {
                u -= Math.Abs(count_num_teaching_class - d);
                Console.WriteLine($"Minus on numClass - u: {u}");
            }
            else
            {
                u += Math.Abs(count_num_teaching_class - d);
                Console.WriteLine($"Plus on numClass - u: {u}");
            }
            return u * (double)pointIndex.AlphaIndex;
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
                var a = _context.RegisterSubjects.FirstOrDefault(x => x.AvailableSubjectId == i && x.UserId == registerSubjectSlot.userId);
                if (a != null)
                {
                    continue;
                }
                
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
            var mess = new String("");
            foreach(var a in registerSubjectSlot.registerSlots)
            {
                var b = _context.RegisterSlots
                    .FirstOrDefault(x => x.UserId == registerSubjectSlot.userId && x.SemesterId == registerSubjectSlot.semesterId && x.Slot == a);
                if (b != null)
                {
                    mess = mess + $",{a}";
                    continue;
                }
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
            if (String.IsNullOrEmpty(mess))
            {
                mess = "Create Success";
            }
            else
            {
                mess = "Slot " + mess + " is already registered";
            }
            return new ObjectResult(mess)
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
        [HttpGet("/api/user/{userId}/semester/{semesterId}/available-subject")]
        [SwaggerOperation(Summary = "Lấy Avaialble Subject list mà User CHƯA đăng ký")]
        public async Task<ObjectResult> getAvailableSubjectByUserId([FromRoute]int userId, [FromRoute] int semesterId)
        {
            var res = await _context.AvailableSubjects
                .Include(x=> x.RegisterSubjects)
                .Where(x => x.SemesterId == semesterId)
                .ToListAsync();
            res = res.Where(x => !x.RegisterSubjects.ToList().Exists(x => x.UserId == userId)).ToList();
            if (res.Count() == 0)
            {
                return new ObjectResult("Lecturer register all subject in this semester");
            }
            else
            {
                return new ObjectResult(res);
            }
        }
        
    }
}
