using Business.AvailableSubjectService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AvailableSubjects
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailableSubjectController : ControllerBase
    {
        private readonly IAvailableSubjectService _availableSubjectService;
        public AvailableSubjectController(IAvailableSubjectService availableSubjectService)
        {
            _availableSubjectService = availableSubjectService;
        }
        [HttpGet("{availableSubjectId}")]
        public async Task<IActionResult> GetAvailableSubjectById(int availableSubjectId)
        {
            var response = await _availableSubjectService.GetAvailableSubjectById(availableSubjectId);
            if(response.StatusCode == 404)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
