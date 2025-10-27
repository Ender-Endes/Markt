using Markt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Markt.Data.Configs;

public class ActivityLogConfig : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> b)
    {
        b.HasIndex(x => new { x.Type, x.CreatedAt });
        b.Property(x => x.Type).HasMaxLength(50).IsRequired();
        b.Property(x => x.Term).HasMaxLength(200);
    }
}