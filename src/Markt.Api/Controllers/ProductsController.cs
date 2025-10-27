using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Markt.Data;
using Markt.Domain.Entities;
using Markt.Domain.DTOs;

namespace Markt.Api.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly MarktDbContext _db;
    public ProductsController(MarktDbContext db) => _db = db;

    /// entity'den DTO dönüşümü
    private static IQueryable<ProductDto> ProjectToDto(IQueryable<Product> q) =>
        q.Select(p => new ProductDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            Price = p.Price,
            BusinessId = p.BusinessId,
            BusinessName = p.Business.DisplayName
        });

    // POST /products
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        var bidStr = User.FindFirstValue("bid");
        if (!int.TryParse(bidStr, out var currentBid))
            return Unauthorized("invalid token");

        if (string.IsNullOrWhiteSpace(product.Title))
            return BadRequest("Title required.");

        product.BusinessId = currentBid;                 
        product.Title = product.Title.Trim();
        product.Description = product.Description?.Trim() ?? string.Empty;

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // Kaydı DTO olarak dön
        var dto = await ProjectToDto(_db.Products.AsNoTracking().Where(p => p.Id == product.Id)).FirstOrDefaultAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, dto);
    }

    // PUT /products/{id}
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Product product)
    {
        var bidStr = User.FindFirstValue("bid");
        if (!int.TryParse(bidStr, out var currentBid))
            return Unauthorized("invalid token");

        if (id != product.Id)
            return BadRequest("Route id and body id must match.");

        var exists = await _db.Products.FirstOrDefaultAsync(e => e.Id == id);
        if (exists is null) return NotFound();

        if (exists.BusinessId != currentBid)
            return Forbid();                               // WHY: sadece sahibi günceller

        if (string.IsNullOrWhiteSpace(product.Title))
            return BadRequest("Title required.");

        exists.Title = product.Title.Trim();
        exists.Description = product.Description?.Trim() ?? string.Empty;
        exists.Price = product.Price;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /products/{id}
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var bidStr = User.FindFirstValue("bid");
        if (!int.TryParse(bidStr, out var currentBid))
            return Unauthorized("invalid token");

        var prod = await _db.Products.FindAsync(id);
        if (prod is null) return NotFound();

        if (prod.BusinessId != currentBid)
            return Forbid();                              

        _db.Products.Remove(prod);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET /products/count?businessId=1
    [HttpGet("count")]
    public async Task<IActionResult> Count([FromQuery] int? businessId = null)
    {
        if (businessId is null)
            return BadRequest("businessId must be provided");

        var count = await _db.Products.AsNoTracking()
            .CountAsync(e => e.BusinessId == businessId.Value);
        return Ok(count);
    }

    // GET /products?businessId=1
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? businessId = null)
    {
        var q = _db.Products.AsNoTracking();
        if (businessId is not null)
            q = q.Where(p => p.BusinessId == businessId.Value);

        var list = await ProjectToDto(q).ToListAsync();
        return Ok(list);
    }

    // GET /products/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await ProjectToDto(
        _db.Products.AsNoTracking()
            .Where(p => p.Id == id))
            .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Ok(dto);
    }
}
