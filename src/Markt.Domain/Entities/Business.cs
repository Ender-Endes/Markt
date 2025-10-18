namespace Markt.Domain.Entities;

public class Business
{
    public int Id { get; set; }
    // primary Key: her business’in kendine özel bir kimlik numarası var (her business’in kendi id’si oluyor).

    public int UserId { get; set; }
    // foreign Key: bu business’i kimin sahip olduğunu gösteriyor (business’i user’a bağlıyor)

    public string DisplayName { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;

    public User User { get; set; } = default!;
    // burası her zaman dolu çünkü her business mutlaka bir user'a bağlı olmalı
    // sahipsiz business olmamalı
    public List<Product> Products { get; set; } = new();
    // işletmenin product listesi (varsayılan boş başlar)
}


