namespace Markt.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    // ürünün kendi kimliği - unique id

    public int BusinessId { get; set; }
    // ait olduğu işletmenin id'si - foreign key

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public Business Business { get; set; } = default!;


}
