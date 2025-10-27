using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Markt.Data;
using Markt.Domain.Entities;

namespace Markt.Api.Controllers;

[ApiController]
[Route("offers")]
public class OffersController : ControllerBase
{
    private readonly MarktDbContext _db;
    public OffersController(MarktDbContext db) => _db = db;

    // POST /offers
    // body: { "fromBusinessId":1, "toBusinessId":2, "productId":null, "message":"fiyat teklifi" }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Offer offer)
    {
        // self offer engeli
        if (offer.FromBusinessId == offer.ToBusinessId)
            return BadRequest("Cannot send an offer to yourself.");

        // offer eden ve edilen işletme mevcut mu
        var fromOk = await _db.Businesses.AsNoTracking().AnyAsync(b => b.Id == offer.FromBusinessId);
        var toOk   = await _db.Businesses.AsNoTracking().AnyAsync(b => b.Id == offer.ToBusinessId);
        if (!fromOk || !toOk) return BadRequest("Invalid FromBusinessId or ToBusinessId.");

        // productId verilmişse, toBusinessId'ye ait mi kontrol et
        if (offer.ProductId is not null)
        {
            var prodOk = await _db.Products.AsNoTracking()
                .AnyAsync(p => p.Id == offer.ProductId && p.BusinessId == offer.ToBusinessId);
            if (!prodOk) return BadRequest("Product not found for the target business.");
        }

        // bütün gereklilikler yerine gelince pending status ile kaydet
        offer.Status = string.IsNullOrWhiteSpace(offer.Status) ? "Pending" : offer.Status.Trim();

        // oluşturulma zamanını kaydet
        offer.CreatedAt = DateTime.UtcNow;

        _db.Offers.Add(offer);
        await _db.SaveChangesAsync();
        return Created($"/offers/{offer.Id}", offer);
    }

    // GET /offers/inbox?toBusinessId=2
    [HttpGet("inbox")]
    public async Task<IActionResult> Inbox([FromQuery] int toBusinessId)
    {
        var list = await _db.Offers.AsNoTracking()
            .Where(x => x.ToBusinessId == toBusinessId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return Ok(list);
    }

    // GET /offers/outbox?fromBusinessId=1
    [HttpGet("outbox")]
    public async Task<IActionResult> Outbox([FromQuery] int fromBusinessId)
    {
        var list = await _db.Offers.AsNoTracking()
            .Where(x => x.FromBusinessId == fromBusinessId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return Ok(list);
    }

    // PUT /offers/{id}   body: { "status": "Accepted" }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] Offer body)
    {
        var offer = await _db.Offers.FirstOrDefaultAsync(x => x.Id == id);
        if (offer is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(body.Status))
            offer.Status = body.Status.Trim();

        if (body.ProductId != offer.ProductId && body.ProductId is not null)
        {
            // İsteğe bağlı: ProductId update’ine de izin veriyorsan doğrula
            var prodOk = await _db.Products.AsNoTracking()
                .AnyAsync(p => p.Id == body.ProductId && p.BusinessId == offer.ToBusinessId);
            if (!prodOk) return BadRequest("Product not found for the target business.");
            offer.ProductId = body.ProductId;
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
