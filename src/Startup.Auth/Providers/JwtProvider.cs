using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Startup.Auth.Models;

namespace Startup.Auth.Provider
{
    public interface IJwtProvider
    {
        string GenerateToken(UserModel user);
    }
    public class JwtProvider : IJwtProvider
    {
        private readonly IApplicationSettings _applicationSettings;
        public JwtProvider(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }
        public string GenerateToken(UserModel user)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_applicationSettings.Secret));

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Type),
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}