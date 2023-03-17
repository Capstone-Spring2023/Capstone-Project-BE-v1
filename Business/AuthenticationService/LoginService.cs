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
        public LoginService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string CreateToken()
        {
            var claims = new[]
              {
                    //new Claim(JwtRegisteredClaimNames.Sub, Security.Encrypt(AppSettings.appSettings.JwtEmailEncryption,user.Gmail)),
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, "Dat"),
                    new Claim(ClaimTypes.Email,"tqdatqn01230@gmail.com"),
                    new Claim(ClaimTypes.Role, "User")
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:JwtSecret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(String.Empty,
              String.Empty,
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
                return CreateToken();
            }catch (InvalidJwtException exception)
            {
                return "Invalid Token";
            }
        }
    }
}
