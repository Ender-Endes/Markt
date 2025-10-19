// BusinessConfig.cs
using Markt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Markt.Data.Configs;

public class BusinessConfig : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        // business - her bizness'in bir user'i var, her user'in de en fazla bir bizness'i olabilir. 

        builder.HasOne(x => x.User).WithOne(x => x.Business).HasForeignKey<Business>(x => x.UserId);
        builder.HasMany(x => x.Products).WithOne(x => x.Business).HasForeignKey(x => x.BusinessId);
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        // displayname hızlı search için indexlendi
        builder.HasIndex(x => x.DisplayName);
    }
}
