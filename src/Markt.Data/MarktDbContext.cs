namespace Markt.Data;

using Microsoft.EntityFrameworkCore;
using Markt.Domain.Entities;

public class MarktDbContext : DbContext
{
    public MarktDbContext(DbContextOptions<MarktDbContext> options) : base(options) { }

    public DbSet<Business>     Businesses   => Set<Business>();
    public DbSet<Product>      Products     => Set<Product>();
    public DbSet<Post>         Posts        => Set<Post>();
    public DbSet<Comment>      Comments     => Set<Comment>();
    public DbSet<Like>         Likes        => Set<Like>();         
    public DbSet<Offer>        Offers       => Set<Offer>();
    public DbSet<Message>      Messages     => Set<Message>();        
    public DbSet<ActivityLog>  ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(MarktDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}
