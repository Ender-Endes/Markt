using Microsoft.EntityFrameworkCore;
using Markt.Domain.Entities;

namespace Markt.Data;

public class MarktDbContext : DbContext
{
    public MarktDbContext(DbContextOptions<MarktDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // user email unique ve dolu olmalı
        var user = builder.Entity<User>();
        user.HasIndex(e => e.Email).IsUnique();
        user.Property(e => e.Email).IsRequired().HasMaxLength(200);   

        // business - her bizness'in bir user'i var, her user'in de en fazla bir bizness'i olabilir. 
        var biz = builder.Entity<Business>();
        biz.HasOne(e => e.User)
        .WithOne(e => e.Business)
        .HasForeignKey<Business>(e => e.UserId);

        biz.HasMany(e => e.Products)
        .WithOne(e => e.Business)
        .HasForeignKey(e => e.BusinessId);

        // displayname daha hızlı search için index'lendi
        biz.HasIndex(e => e.DisplayName);
        biz.Property(e => e.DisplayName).HasMaxLength(200).IsRequired();



        // Product (Business 1-n Product)
        var prod = builder.Entity<Product>();
        prod.Property(e => e.Price).HasColumnType("decimal(18,2)");
        // ürün adı üzerinden arama hızlı olsun diye index'lendi.
        prod.HasIndex(e => e.Title);
        prod.Property(e => e.Title).HasMaxLength(200).IsRequired();

        base.OnModelCreating(builder);

    }
}
