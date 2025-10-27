namespace Markt.Domain.Entities;

public class Message
{
    public int Id { get; set; }
    public int FromBusinessId { get; set; }
    public int ToBusinessId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}