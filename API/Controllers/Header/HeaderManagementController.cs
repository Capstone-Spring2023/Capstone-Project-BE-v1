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
    [Authorize]
    public class HeaderManagementController : ControllerBase
    {
        private readonly IUserService _userService;

        public HeaderManagementController(IUserService userService)
        {
            _userService = userService;
        }

        //GET: api/User/id
        [HttpGet]
        [Route("api/header/profile/getHeader/{id}")]
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
        [Route("api/header/profile/updateHeader/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserModel user)
        {
            var response = await _userService.UpdateUser(id, user);
            if(response.StatusCode == 400)
            {
                return NotFound();
            }
            return Ok(response.Data);
        }

        [HttpGet]
        [Route("api/header/leaderManagement/getAllLeaders")]
        public async Task<IActionResult> GetAllLeaders()
        {
            var listLeader = await _userService.GetAllLeaders();
            if(listLeader == null)
            {
                return NotFound();
            }
            return Ok(listLeader);
        }
    }


    
}
