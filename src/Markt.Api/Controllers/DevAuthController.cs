using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Markt.Api.Controllers;

[ApiController]
[Route("auth")]
public class DevAuthController : ControllerBase
{
    private readonly IConfiguration _cfg;
    public DevAuthController(IConfiguration cfg) => _cfg = cfg;

    // GET /auth/dev-token?businessId=1
    [HttpGet("dev-token")]
    public IActionResult DevToken([FromQuery] int businessId)
    {
        var jwt = _cfg.GetSection("JwtConfig");
        var secret = jwt["Secret"];
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
            return Problem("JwtConfig.Secret must be set (min 32 chars).");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("bid", businessId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, businessId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, $"dev{businessId}@demo.local"),
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { token = jwtString });
    }
}
