using Markt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Markt.Data.Configs;

public class BusinessConfig : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        // temel alanlar
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired(); 
        builder.Property(x => x.Sector).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Phone).HasMaxLength(50);

        // indexler (arama + benzersizlik)
        builder.HasIndex(x => x.DisplayName);      

        // aynı e-mail ile ikinci işletme olmasın

        builder.HasIndex(x => x.Email).IsUnique();  

        // default değer
        builder.Property(x => x.IsApproved).HasDefaultValue(true);
    }
}
