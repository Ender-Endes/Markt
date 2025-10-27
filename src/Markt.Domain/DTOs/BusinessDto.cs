namespace Markt.Domain.DTOs;

public class BusinessDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // ürün listesini buraya dahil etme, sadece özet için id’leri al
    public List<int>? ProductIds { get; set; } = [];
}
