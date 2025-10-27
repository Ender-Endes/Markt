namespace Markt.Domain.Entities;

public class Like
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int BusinessId { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Post Post { get; set; } = default!;
    public Business Business { get; set; } = default!;
}