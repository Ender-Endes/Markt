using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Markt.Data;
using Markt.Domain.Entities;
using Markt.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace Markt.Api.Controllers;

[ApiController]
[Route("businesses")]
public class BusinessesController : ControllerBase
{
    private readonly MarktDbContext _db;
    public BusinessesController(MarktDbContext db) => _db = db;

    // 
    private static IQueryable<BusinessDto> ProjectToDto(IQueryable<Business> q, bool includeProductIds)
    {
        if (includeProductIds)
        {
            return q.Select(b => new BusinessDto
            {
                Id = b.Id,
                DisplayName = b.DisplayName,
                Sector = b.Sector,
                City = b.City,
                Phone = b.Phone,
                ProductIds = b.Products.Select(p => p.Id).ToList()
            });
        }

        return q.Select(b => new BusinessDto
        {
            Id = b.Id,
            DisplayName = b.DisplayName,
            Sector = b.Sector,
            City = b.City,
            Phone = b.Phone,
            ProductIds = null
        });
    }

    // GET /businesses?q=ali&includeProductIds=false
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? q = null, [FromQuery] bool includeProductIds = false)
    {
        var query = _db.Businesses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(b =>
                EF.Functions.Like(b.DisplayName, $"%{term}%") ||
                EF.Functions.Like(b.Sector, $"%{term}%") ||
                EF.Functions.Like(b.City, $"%{term}%"));
        }

        query = query.OrderBy(b => b.DisplayName);

        var items = await ProjectToDto(query, includeProductIds).ToListAsync();
        return Ok(items);
    }

    // GET /businesses/{id}?includeProductIds=true
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool includeProductIds = false)
    {
        var dto = await ProjectToDto(
                        _db.Businesses.AsNoTracking().Where(b => b.Id == id),
                        includeProductIds)
                    .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Ok(dto);
    }

    // POST /businesses
    // body: { "displayName":"Ali Ofis", "email":"ali@demo.com", "passwordHash":"...", "sector":"Mobilya", "city":"İstanbul", "phone":"...", "about":"..." }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Business b)
    {
        if (string.IsNullOrWhiteSpace(b.DisplayName)) return BadRequest("DisplayName required.");
        if (string.IsNullOrWhiteSpace(b.Email)) return BadRequest("Email required.");
        if (string.IsNullOrWhiteSpace(b.PasswordHash)) return BadRequest("PasswordHash required.");

        b.DisplayName = b.DisplayName.Trim();
        b.Email = b.Email.Trim();
        b.Sector = b.Sector?.Trim() ?? string.Empty;
        b.City = b.City?.Trim() ?? string.Empty;
        b.Phone = b.Phone?.Trim() ?? string.Empty;
        b.About = b.About?.Trim() ?? string.Empty;

        var existsEmail = await _db.Businesses.AsNoTracking().AnyAsync(x => x.Email == b.Email);
        if (existsEmail) return BadRequest("Email already exists.");

        _db.Businesses.Add(b);
        await _db.SaveChangesAsync();

        var dto = await ProjectToDto(_db.Businesses.AsNoTracking().Where(x => x.Id == b.Id), includeProductIds: false).FirstOrDefaultAsync();
        return CreatedAtAction(nameof(GetById), new { id = b.Id }, dto);
    }

    // PUT /businesses/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Business body)
    {
        // id kontrolü
        if (id != body.Id) return BadRequest("Route id and body id must match.");

        // kayıt mevcut mu
        var b = await _db.Businesses.FirstOrDefaultAsync(x => x.Id == id);
        if (b is null) return NotFound();

        // zorunlu alanlar doldurulmuş mu
        if (string.IsNullOrWhiteSpace(body.DisplayName)) return BadRequest("DisplayName required.");
        if (string.IsNullOrWhiteSpace(body.Email)) return BadRequest("Email required.");

        // Email unique mi
        var emailUsedByAnother = await _db.Businesses.AsNoTracking()
            .AnyAsync(x => x.Email == body.Email && x.Id != id);
        if (emailUsedByAnother) return BadRequest("Email already in use.");

        // trimle ve update et
        b.DisplayName = body.DisplayName.Trim();
        b.Email = body.Email.Trim();
        b.Sector = body.Sector?.Trim() ?? string.Empty;
        b.City = body.City?.Trim() ?? string.Empty;
        b.Phone = body.Phone?.Trim() ?? string.Empty;
        b.About = body.About?.Trim() ?? string.Empty;

        // Parola alanı opsiyonel güncellenir (boş gelirse dokunma)
        if (!string.IsNullOrWhiteSpace(body.PasswordHash))
            b.PasswordHash = body.PasswordHash.Trim();

        // onay durumu
        b.IsApproved = body.IsApproved;

        await _db.SaveChangesAsync();

        var dto = await ProjectToDto(_db.Businesses.AsNoTracking().Where(x => x.Id == b.Id), includeProductIds: false).FirstOrDefaultAsync();
        return Ok(dto);
    }
    
    // Admin'in onay vermesi
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromQuery] bool value = true)
    {
        var b = await _db.Businesses.FirstOrDefaultAsync(x => x.Id == id);
        if (b is null) return NotFound();
        b.IsApproved = value;
        await _db.SaveChangesAsync();
        return Ok(new { id = b.Id, isApproved = b.IsApproved });
}
}
