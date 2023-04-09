using Business.RegisterSubjectService.Interfaces;
using Business.RegisterSubjectService.Models;
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
        private readonly IRegisterSubjectService _registerSubjectService;

        public RegisterSubjectController(IRegisterSubjectService registerSubjectService)
        {
            _registerSubjectService = registerSubjectService;
        }

        [HttpPost]
        [Route("/api/lecture/registerSubject/createRegisterSubject")]
        public async Task<IActionResult> CreateRegisterSubject(CreateRegisterSubjectModel model)
        {
            var response = await _registerSubjectService.CreateRegisterSubject(model);
            return Ok(response);
        }
    }
}
