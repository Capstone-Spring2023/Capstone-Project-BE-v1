using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AbleSubjectResponse
    {
        public int subjectId { get; set; }  
        public string subjectName { get; set; }
    }
    public class AbleSubjectRequest
    {
        public int subjectId { get; set; }
        public int userId { get; set; }
    }
    [Route("api/able-subject")]
    [ApiController]
    public class AbleSubjectController : ControllerBase
    {
        private readonly CFManagementContext _context;
        public AbleSubjectController(CFManagementContext cF)
        {
            _context = cF;
        }
        [HttpGet("/api/user/{userId}/able-subject")]
        public ObjectResult getAbleSubject(int userId)
        {
            var a = _context.Users
                .Include(x => x.Subjects)
                .First(x => x.UserId == userId);
            var res = a.Subjects.Select(x => new AbleSubjectResponse()
            {
                subjectId = x.SubjectId,
                subjectName = x.SubjectName,
            });
            return new ObjectResult(res);
        }
        [HttpGet]
        public ObjectResult getAbleSubject()
        {
            var res = _context.Subjects.Select(_x => new AbleSubjectResponse()
            {
                subjectId = _x.SubjectId,
                subjectName = _x.SubjectName,
            });
            return new ObjectResult(res);
        }
        [HttpPost("api/user/able-subject")]
        public async Task<ObjectResult> createAbleSubject(AbleSubjectRequest request)
        {
            var a = _context.Users
                .Include(x => x.Subjects)
                .First(x => x.UserId == request.userId);
            if (!a.Subjects.Select(x=> x.SubjectId).Contains(request.subjectId))
            {
                var subject = _context.Subjects.First(x=> x.SubjectId == request.subjectId);    
                a.Subjects.Add(subject);

            }
            await _context.SaveChangesAsync();
            return new ObjectResult("Created");
        }

    }
}
