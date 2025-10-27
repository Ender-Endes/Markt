namespace Markt.Domain.Entities;

public class Post
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Business Business { get; set; } = default!;
    public List<Comment> Comments { get; set; } = [];
    public List<Like> Likes { get; set; } = [];
}