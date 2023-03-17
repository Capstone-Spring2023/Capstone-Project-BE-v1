using Business.AvailableSubjectService.Interface;
using Data.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AvaibleSubjectController
{
    [Route("v1/api/available-subjects")]
    [ApiController]
    public class AvailableSubjectController : ControllerBase
    {
        private readonly int pageSize = 10;
        private readonly IAvailableSubjectService avaibleSubjectService;
        public AvailableSubjectController(IAvailableSubjectService availableSubjectService)
        {
            this.avaibleSubjectService = availableSubjectService;
        }

        [HttpGet("")]
        public async Task<ObjectResult> GetAll([FromQuery] int pageIndex)
        {
            PagingRequest pagingRequest = new PagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var response = await avaibleSubjectService.GetAvailableSubjects(x => true, pagingRequest);
            return response;
        }
        [HttpGet("teachers/subject/{subjectId}")]
        public async Task<ObjectResult> GetTeachers([FromRoute] int subjectId, [FromQuery] int pageIndex)
        {
            PagingRequest pagingRequest = new PagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            return await avaibleSubjectService.GetTeachersBySubjectId(subjectId, pagingRequest);
        }
        [HttpGet]
        [Route("api/availableSubject/getAllAvailableSubjectByLeaderId/{leaderId}")]
        public async Task<IActionResult> getAllAvailableSubjectByLeaderId(int leaderId)
        {
            var response = await avaibleSubjectService.GetAllAvailableSubjectByLeaderId(leaderId);
            if (response.StatusCode == (int)Business.Constants.StatusCode.NOTFOUND)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
