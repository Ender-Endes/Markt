namespace Markt.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    //admin kayıttan sonra üyelere approve vermeli
    public bool Approved { get; set; } = false;
    public string Role { get; set; } = "Member";

    public Business? Business { get; set; }
    // burası ? çünkü user'ın her zaman business'i olmak zorunda değil
    // yani yeni kayıt olan bir user henüz pazaryeri açmamış olabilir
}   
