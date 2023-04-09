using Business.AuthenticationService;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Repositories.Interface;
using Business.UserService.Interfaces;
using Business.UserService.Models;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    public class LeaderManagementController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly CFManagementContext _context;

        public LeaderManagementController(IUserService userService, CFManagementContext context)
        {
            _context = context;
            _userService = userService;
        }

        //GET: api/User/id
        [HttpGet]
        [Route("api/leader/profile/getLeader/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var response = await _userService.GetUser(id);
            if(response.StatusCode == 400)
            {
                return NotFound();
            }
            return Ok(response);
            
        }

        //PUT: api/User/
        [HttpPut]
        [Route("api/leader/profile/updateLeader/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserModel user)
        {
            var response = await _userService.UpdateUser(id, user);
            if(response.StatusCode == 400)
            {
                return NotFound();
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("api/leader")]
        public async Task<ObjectResult> CreateLeader([FromBody] LeaderCreateRequest request)
        {
            var aSubject = _context.AvailableSubjects.Find(request.AvailableSubjectId);
            aSubject.LeaderId = request.UserId;
            var user = _context.Users.Find(request.UserId);
            aSubject.LeaderName = user.FullName;
            user.RoleId = 3;
            await _context.SaveChangesAsync();
            return new ObjectResult("Create Success")
            {
                StatusCode = 201,
            };
        }
        [HttpPut]
        [Route("api/leader")]
        public async Task<ObjectResult> UpdateLeader([FromBody] LeaderCreateRequest request)
        {
            var aSubject = _context.AvailableSubjects.Find(request.AvailableSubjectId);
            
            var newLeader = _context.Users.Find(request.UserId);
            var oldLeader = _context.Users.Find(aSubject.LeaderId);
            aSubject.LeaderId = request.UserId;
            if (oldLeader == null)
            {
                return new ObjectResult("Subject haven't have leader yet")
                {
                    StatusCode = 400,
                };
            }
            aSubject.LeaderName = newLeader.FullName;

            newLeader.RoleId = 3;
            oldLeader.RoleId = 4;

            await _context.SaveChangesAsync();
            return new ObjectResult("Update Success")
            {
                StatusCode = 200,
            };
        }
    }


    
}
