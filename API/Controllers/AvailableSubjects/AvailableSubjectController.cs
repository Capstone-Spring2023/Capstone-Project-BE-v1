using Business.AvailableSubjectService.Interface;
using Data.Paging;
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

        [HttpGet("/Department/{departmentId}")]
        public async Task<IActionResult> GetAvailableSubjectsByDepartmentId(int departmentId)
        {
            var response = await _availableSubjectService.GetAvailableSubjectByDepartmentId(departmentId);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAvailableSubjects()
        {
            PagingRequest pagingRequest = new PagingRequest()
            {
                PageIndex = 1,
                PageSize = 20
            };
            var response = await _availableSubjectService.GetAvailableSubjects(x => x.Status == true, pagingRequest);
            return Ok(response.Value);
        }
    }
}
