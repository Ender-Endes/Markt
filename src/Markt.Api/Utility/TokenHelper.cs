using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Markt.Domain.Entities;

namespace Markt.Api.Utility
{
    public static class TokenHelper
    {
        public static string CreateToken(Business b, string issuer, string audience, string secret, int minutes)
        {
            var claims = new List<Claim>
            {
                new Claim("bid", b.Id.ToString()),
                new Claim("approved", b.IsApproved ? "1" : "0"),
                new Claim(ClaimTypes.Name, b.DisplayName ?? b.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
