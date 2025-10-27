using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Markt.Data;
using Markt.Domain.Entities;

namespace Markt.Api.Controllers;

[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{
    private readonly MarktDbContext _db;
    public SearchController(MarktDbContext db) => _db = db;

    // GET /search?q=koltuk
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var term = (q ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(term)) return BadRequest("q required");

        var products = await _db.Products.AsNoTracking()
            .Where(p => EF.Functions.Like(p.Title, $"%{term}%") || EF.Functions.Like(p.Description, $"%{term}%"))
            .OrderBy(p => p.Title)
            .Take(50)
            .ToListAsync();

        var businesses = await _db.Businesses.AsNoTracking()
            .Where(b => EF.Functions.Like(b.DisplayName, $"%{term}%") || EF.Functions.Like(b.Sector, $"%{term}%"))
            .OrderBy(b => b.DisplayName)
            .Take(50)
            .ToListAsync();

        _db.ActivityLogs.Add(new ActivityLog { Type = "Search", Term = term });
        await _db.SaveChangesAsync();

        return Ok(new { products, businesses });
    }
}