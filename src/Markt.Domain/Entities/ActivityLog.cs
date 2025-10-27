namespace Markt.Domain.Entities;

public class ActivityLog
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; 
    public string? Term { get; set; }  
    public int? BusinessId { get; set; } 
    public string? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}