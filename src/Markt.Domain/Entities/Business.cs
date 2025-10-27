namespace Markt.Domain.Entities;

public class Business
{
    public int Id { get; set; }
    // primary Key: her business’in kendine özel bir kimlik numarası var (her business’in kendi id’si oluyor).


    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; 
    public bool IsApproved { get; set; } = true;

    public string Sector { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = [];
    // işletmenin product listesi (varsayılan boş başlar)
}


