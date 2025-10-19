namespace Markt.Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName  { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public Business? Business { get; set; } 
    // her user’ın en fazla 1 business’i

    }
