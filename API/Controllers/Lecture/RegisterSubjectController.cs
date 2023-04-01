using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Lecture
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterSubjectController : ControllerBase
    {
        private readonly CFManagementContext _context;

        public RegisterSubjectController(CFManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/lecture/registerSubject/getRegisterSubject/{id}")]
        public  IActionResult getRegisterSubject()
        {
            var respone = 123;
            return Ok(respone);
        }
    }
}
