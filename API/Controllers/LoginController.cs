using Microsoft.AspNetCore.Mvc;
using Business.AuthenticationService;
using API.Model;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("v1/api/authentication")]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _loginService;
        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }
        
        [HttpPost("login-google")]
        public IActionResult LoginGoogle(UserView userView)
        {
            var response = _loginService.CheckValidateGoogleToken(userView);
            return Ok(response.Result);
        }

        [HttpGet("test-authen")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public string test()
        {
            return "OK";
        }
    }
}
