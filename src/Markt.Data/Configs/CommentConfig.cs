using Markt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Markt.Data.Configs;

public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> b)
    {
        b.Property(x => x.Text).HasMaxLength(500).IsRequired();

        b.HasIndex(x => new { x.PostId, x.CreatedAt });

        b.HasOne(x => x.Post)
         .WithMany(p => p.Comments)
         .HasForeignKey(x => x.PostId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Business)
         .WithMany() 
         .HasForeignKey(x => x.BusinessId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
