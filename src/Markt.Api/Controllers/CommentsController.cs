using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Markt.Data;
using Markt.Domain.Entities;
using System.Security.Claims;

namespace Markt.Api.Controllers;

[ApiController]
[Route("comments")]
public class CommentsController : ControllerBase
{
    private readonly MarktDbContext _db;
    public CommentsController(MarktDbContext db) => _db = db;

    // POST /comments
    // Body: { postId, text }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Comment c)
    {
        // kim
        var bidStr = User.FindFirstValue("bid");
        if (!int.TryParse(bidStr, out var currentBid))
            return Unauthorized("invalid token");

        // basit doğrulama
        if (string.IsNullOrWhiteSpace(c.Text))
            return BadRequest("text is required");
        c.Text = c.Text.Trim();

        // post var mı?
        var postExists = await _db.Posts.AsNoTracking().AnyAsync(p => p.Id == c.PostId);
        if (!postExists) return BadRequest("post not found");

        // yorumu yazan business
        c.BusinessId = currentBid;
        c.CreatedAt = DateTime.UtcNow;

        _db.Comments.Add(c);
        await _db.SaveChangesAsync();

        return Created($"/comments/{c.Id}", c);
    }

    // DELETE /comments/{id}
    // yalnızca yorumu yazan silebilsin 
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var bidStr = User.FindFirstValue("bid");
        if (!int.TryParse(bidStr, out var currentBid))
            return Unauthorized("invalid token");

        var c = await _db.Comments.FirstOrDefaultAsync(x => x.Id == id);
        if (c is null) return NotFound();

        // postu silen kişi, postu yazan kişi olmalı
        if (c.BusinessId != currentBid)
            return Forbid("you can delete only your own comment");

        _db.Comments.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
