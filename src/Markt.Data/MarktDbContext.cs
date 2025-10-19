namespace Markt.Data;


using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Markt.Domain.Entities;

public class MarktDbContext : IdentityDbContext<AppUser>
{
    public MarktDbContext(DbContextOptions<MarktDbContext> options) : base(options) { }

    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(MarktDbContext).Assembly);
        base.OnModelCreating(builder);
    }

}
