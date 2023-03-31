using Data.Models;
using Google.Apis.Auth;
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
        public string CreateToken(GoogleJsonWebSignature.Payload payload)
        {
            var user = context.Users.Where(x => x.Email == payload.Email).FirstOrDefault();
            var roleName = context.Roles.Where(x => x.RoleId == user.RoleId).FirstOrDefault().RoleName;
            var claims = new[]
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
            string tokenId = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenId;
        }
        public string CheckValidateGoogleToken(UserView userView)
        {
            try
            {

                var payload = GoogleJsonWebSignature.ValidateAsync(userView.tokenId, new GoogleJsonWebSignature.ValidationSettings()).Result;
                return CreateToken(payload);
            }catch (InvalidJwtException exception)
            {
                return "Invalid Token";
            }
        }
    }
}
