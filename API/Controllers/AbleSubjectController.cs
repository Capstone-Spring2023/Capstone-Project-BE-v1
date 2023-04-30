using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Business.AbleSubjectService;
using Business.AbleSubjectService.Interface;
using Business.AbleSubjectService.Models;

namespace API.Controllers
{
    [Route("api/able-subject")]
    [ApiController]
    public class AbleSubjectController : ControllerBase
    {
        //private readonly CFManagementContext _context;
        private readonly IAbleSubjectService _ableSubjectService;
        public AbleSubjectController(IAbleSubjectService ableSubjectService)
        {
            _ableSubjectService = ableSubjectService;
        }
        [HttpGet("/api/user/{userId}/able-subject")]
        public async Task<ObjectResult> getAbleSubject(int userId)
        {
            var res = await _ableSubjectService.getAbleSubjectsByUserId(userId);
            return res;
        }
        [HttpGet]
        public async Task<ObjectResult> getAbleSubject()
        {
            var res = await _ableSubjectService.getAbleSubjects();
            return res;
        }
        [HttpPost("api/user/able-subject")]
        public async Task<ObjectResult> createAbleSubject(AbleSubjectRequest request)
        {
            var res = await _ableSubjectService.createAbleSubject(request);
            return res;
        }
        [HttpDelete("api/user/able-subject")]
        public async Task<ObjectResult> deleteAbleSubject(AbleSubjectRequest request)
        {
            var res = await _ableSubjectService.deleteAbleSubject(request);
            return res;
        }

    }
}
