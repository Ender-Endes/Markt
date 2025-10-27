using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Markt.Data;
using Markt.Domain.Entities;
using System.Security.Claims;

namespace Markt.Api.Controllers;

[ApiController]
[Route("likes")]
public class LikesController : ControllerBase
{
    private readonly MarktDbContext _db;
    public LikesController(MarktDbContext db) => _db = db;

    // POST /likes/toggle/{postId}
    [Authorize]
    [HttpPost("toggle/{postId:int}")]
    public async Task<IActionResult> Toggle(int postId)
    {
        // like'in sahibi kim
        var bidStr = User.FindFirstValue("bid");
        if (!int.TryParse(bidStr, out var currentBid))
            return Unauthorized("invalid token");

        // post var mı
        var postExists = await _db.Posts.AsNoTracking().AnyAsync(p => p.Id == postId);
        if (!postExists) return BadRequest("post not found");

        // mevcut like var mı
        var existing = await _db.Likes.FirstOrDefaultAsync(x => x.PostId == postId && x.BusinessId == currentBid);


        // like yok ise ekle - toggle et
        if (existing is null)
        {
            // ekle
            _db.Likes.Add(new Like
            {
                PostId = postId,
                BusinessId = currentBid,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            // like varsa kaldır
            _db.Likes.Remove(existing);
        }

        await _db.SaveChangesAsync();

        // istatistik için like sayısını dön - like post'un sahibine yazar
        var count = await _db.Likes.AsNoTracking().CountAsync(x => x.PostId == postId);
        return Ok(new { postId, likes = count, liked = (existing is null) });
    }
}
