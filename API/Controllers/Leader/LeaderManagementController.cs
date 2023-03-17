using Business.AuthenticationService;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Repositories.Interface;
using Business.UserService.Interfaces;
using Business.UserService.Models;

namespace API.Controllers
{
    [ApiController]
    public class LeaderManagementController : ControllerBase
    {
        private readonly IUserService _userService;

        public LeaderManagementController(IUserService userService)
        {
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
    }


    
}
