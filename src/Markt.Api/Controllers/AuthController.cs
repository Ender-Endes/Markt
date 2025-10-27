using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Markt.Data;
using Markt.Api.Utility; 
using Markt.Domain.Entities;

namespace Markt.Api.Controllers;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MarktDbContext _db;
    private readonly IConfiguration _cfg;

    public AuthController(MarktDbContext db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto req)
    {
        // alanlar dolu olmalı
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Email and password are required.");

        // böyle bir business var mı
        var biz = await _db.Businesses.FirstOrDefaultAsync(b => b.Email == req.Email);
        if (biz is null) return Unauthorized("Business not found.");

        // şifre doğru mu
        if (biz.PasswordHash != req.Password) return Unauthorized("Wrong password.");


        var jwt = _cfg.GetSection("JwtConfig");
        var token = TokenHelper.CreateToken(
            biz,
            issuer: jwt["Issuer"]!,
            audience: jwt["Audience"]!,
            secret: jwt["Secret"]!,
            minutes: int.TryParse(jwt["AccessTokenExpiration"], out var m) ? m : 30
        );

        return Ok(new { token });
    }
}

public class LoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
