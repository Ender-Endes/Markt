using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Markt.Data;
using Markt.Domain.Entities;
using System.Security.Claims;

namespace Markt.Api.Controllers;

[ApiController]
[Route("posts")]
public class PostsController : ControllerBase
{
    private readonly MarktDbContext _db;
    public PostsController(MarktDbContext db) => _db = db;

    // GET /posts?businessId=1
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? businessId = null)
    {
        var q = _db.Set<Post>().AsNoTracking().OrderByDescending(p => p.CreatedAt).AsQueryable();
        if (businessId is not null) q = q.Where(p => p.BusinessId == businessId.Value);
        return Ok(await q.ToListAsync());
    }

    // POST /posts
    // body: { "title":"Kampanya", "content":"..." }   // businessId artık body’den alınmıyor
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Post post)
    {
        // token’daki işletme id’sini al
        var bidStr = User.FindFirstValue("bid");
        if (!int.TryParse(bidStr, out var currentBid))
            return Unauthorized("invalid token");

        if (string.IsNullOrWhiteSpace(post.Title)) return BadRequest("Title required.");

        post.Title = post.Title.Trim();
        post.Content = post.Content?.Trim() ?? string.Empty;

        // güvenlik: body’de gelse bile görmezden geliyoruz
        post.BusinessId = currentBid;
        post.CreatedAt = DateTime.UtcNow;

        _db.Set<Post>().Add(post);
        await _db.SaveChangesAsync();

        // basit Created, GetById yoksa sorun çıkarmaz
        return Created($"/posts/{post.Id}", post);
    }

    // DELETE /posts/{id}
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        // token’daki işletme id’si
        var bidStr = User.FindFirstValue("bid");
        if (!int.TryParse(bidStr, out var currentBid))
            return Unauthorized("invalid token");

        var post = await _db.Set<Post>().FirstOrDefaultAsync(p => p.Id == id);
        if (post is null) return NotFound();

        // sadece sahibi silebilsin
        if (post.BusinessId != currentBid)
            return Forbid("you can delete only your own post");

        _db.Set<Post>().Remove(post);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
