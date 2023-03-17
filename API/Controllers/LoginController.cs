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
        [HttpGet("login")]
        public TokenReturnModel Login()
        {
            TokenReturnModel tokenReturnModel = new TokenReturnModel()
            {
                Token = _loginService.CreateToken(),
            };
            return tokenReturnModel;
        }
        [HttpPost("login-google")]
        public TokenReturnModel LoginGoogle(UserView userView)
        {
            string token = _loginService.CheckValidateGoogleToken(userView);
            return new TokenReturnModel()
            {
                Token = token,
            };
        }

        [HttpGet("test-authen")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public string test()
        {
            return "OK";
        }
    }
}
