using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UsersService.Models;

namespace UsersService
{
   public static class Auth_JWT
    {
        public static string generateJWT(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            //var jwtSettings = new JwtSettings();
            var key = Encoding.ASCII.GetBytes("secretsecretsecretsecretsecret");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim("username", user.username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static bool valiateJWT(string token)
        {
            return true;
        }
    }
}
