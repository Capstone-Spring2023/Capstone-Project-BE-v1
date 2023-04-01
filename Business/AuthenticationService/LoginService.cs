using Business.AuthenticationService.Models;
using Data.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.AuthenticationService
{
    public class UserView
    {
        public string tokenId { get; set; }
    }
    public class LoginService
    {
        private IConfiguration configuration;
        private readonly CFManagementContext context;
        public LoginService(IConfiguration configuration, CFManagementContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }
        public async Task<ResponseModel> CreateToken(GoogleJsonWebSignature.Payload payload)
        {
            var user = context.Users.Where(x => x.Email == payload.Email).FirstOrDefault();
            if(user == null)
            {
                return new()
                {
                    StatusCode = 500
                };
            }
            var roleName = context.Roles.Where(x => x.RoleId == user.RoleId).FirstOrDefault().RoleName;
            /*var claims = new[]
              {
                    //new Claim(JwtRegisteredClaimNames.Sub, Security.Encrypt(AppSettings.appSettings.JwtEmailEncryption,user.Gmail)),
                    new Claim("Email", user.Email.Trim()),
                    new Claim("Role", roleName.Trim()),
                    new Claim("UserId", user.UserId.ToString()),                   
               };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:JwtSecret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
              configuration["AppSettings:Issuer"],
              configuration["AppSettings:Audience"],
              claims,
              expires: DateTime.Now.AddSeconds(55 * 60),
              signingCredentials: creds);
            string tokenId = new JwtSecurityTokenHandler().WriteToken(token);*/

            var responseLogin = new ResponseLogin();
            responseLogin.Email = user.Email.Trim();
            responseLogin.FullName = user.FullName;
            responseLogin.UserId = user.UserId;
            responseLogin.RoleName = roleName.Trim();
            return new()
            {
                StatusCode = 200,
                Data = responseLogin
            };
        }
        public async Task<ResponseModel> CheckValidateGoogleToken(UserView userView)
        {
            try
            {

                var payload = GoogleJsonWebSignature.ValidateAsync(userView.tokenId, new GoogleJsonWebSignature.ValidationSettings()).Result;
                var response = CreateToken(payload);
                return new()
                {
                    StatusCode = 200,
                    Data = response.Result.Data
                };
            }catch (InvalidJwtException exception)
            {
                return new()
                {
                    StatusCode = 500,
                };
            }
        }
    }
}
