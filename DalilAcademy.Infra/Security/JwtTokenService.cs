using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DalilAcademy.Infra.Security
{
    public class JwtTokenService(IOptions<JwtSetting> options) : ITokenService
    {
        public (string Token, DateTime ExpiersAt) CreateToken(User user)
        {
            var jwt = options.Value;
            var expiersAt = DateTime.UtcNow.AddHours(jwt.ExpiresInHours);
            var descritor = new SecurityTokenDescriptor
            {
                Issuer = jwt.Issuer,
                Audience = jwt.Audience,
                Expires = expiersAt,
                Subject = new ClaimsIdentity(
                    [
                     new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                     new Claim(ClaimTypes.Email , user.Email),
                     new Claim(ClaimTypes.Role , user.Role.ToString())
                    ]
                 ),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            return (new JsonWebTokenHandler().CreateToken(descritor), expiersAt);
        }
    }
}
