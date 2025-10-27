using Markt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Markt.Data.Configs;

public class OfferConfig : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Status).HasMaxLength(30).IsRequired();

        // Rapor sorguları için indeksler
        b.HasIndex(x => new { x.ToBusinessId, x.CreatedAt });
        b.HasIndex(x => new { x.FromBusinessId, x.CreatedAt });

        // Basit FK mantığı (navigasyon kullanmıyoruz; kurs uyumlu)
        // ProductId nullable, varsa hedef işletmenin ürününe ait olmalı (controller'da doğrulanır)
    }
}