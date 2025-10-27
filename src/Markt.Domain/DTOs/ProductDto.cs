namespace Markt.Domain.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int BusinessId { get; set; }
    public string? BusinessName { get; set; }    
}
