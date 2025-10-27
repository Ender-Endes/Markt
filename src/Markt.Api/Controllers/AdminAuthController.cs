using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Markt.Api.Controllers;

[ApiController]
[Route("admin")]
public class AdminAuthController : ControllerBase
{
    private readonly IConfiguration _cfg;
    public AdminAuthController(IConfiguration cfg) => _cfg = cfg;

    public class AdminLoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AdminLoginRequest req)
    {
        var a = _cfg.GetSection("Admin");
        if (!string.Equals(req.Email, a["Email"], StringComparison.OrdinalIgnoreCase) ||
            req.Password != a["Password"])
            return Unauthorized("wrong admin credentials");

        var jwt = _cfg.GetSection("JwtConfig");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(JwtRegisteredClaimNames.Sub, "admin"),
            new Claim(JwtRegisteredClaimNames.Email, req.Email),
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { token = tokenStr });
    }
}