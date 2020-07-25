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
        string GenerateToken(Guid userId);
    }
    public class JwtProvider : IJwtProvider
    {
        private readonly IApplicationSettings _applicationSettings;
        public JwtProvider(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }
        public string GenerateToken(Guid userId)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_applicationSettings.Secret));

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}