namespace Markt.Domain.Entities;

public class Offer
{
    public int Id { get; set; }
    public int? ProductId { get; set; } 

    // Teklifi gönderen işletme
    public int FromBusinessId { get; set; } 

    // Teklifi alan işletme
    public int ToBusinessId { get; set; }   
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; 

    public Product? Product { get; set; }
}
