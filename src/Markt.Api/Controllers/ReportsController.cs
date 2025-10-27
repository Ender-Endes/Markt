using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Markt.Data;

namespace Markt.Api.Controllers;

[ApiController]
[Route("reports")]
public class ReportsController : ControllerBase
{
    private readonly MarktDbContext _db;
    public ReportsController(MarktDbContext db) => _db = db;

    // GET /reports/top-offered?take=5
    // En çok teklif alan işletmeler
    [HttpGet("top-offered")]
    public async Task<IActionResult> TopOffered([FromQuery] int take = 5)
    {
        take = Math.Clamp(take, 1, 20);
        var data = await _db.Offers.AsNoTracking()
            .GroupBy(x => x.ToBusinessId)
            .Select(g => new { businessId = g.Key, count = g.Count() })
            .OrderByDescending(x => x.count)
            .Take(take)
            .ToListAsync();

        return Ok(data);
    }

    // GET /reports/top-offerers?take=5
    // En çok teklif gönderen işletmeler
    [HttpGet("top-offerers")]
    public async Task<IActionResult> TopOfferers([FromQuery] int take = 5)
    {
        take = Math.Clamp(take, 1, 20);
        var data = await _db.Offers.AsNoTracking()
            .GroupBy(x => x.FromBusinessId)
            .Select(g => new { businessId = g.Key, count = g.Count() })
            .OrderByDescending(x => x.count)
            .Take(take)
            .ToListAsync();

        return Ok(data);
    }
}