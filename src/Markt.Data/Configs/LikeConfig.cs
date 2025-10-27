using Markt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Markt.Data.Configs;

public class LikeConfig : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> b)
    {
        // 1 postu aynı business iki kere beğenemesin
        b.HasIndex(x => new { x.PostId, x.BusinessId }).IsUnique(); 

        // post - business ilişkileri
        b.HasOne(x => x.Post)
         .WithMany(p => p.Likes)
         .HasForeignKey(x => x.PostId)
         .OnDelete(DeleteBehavior.Cascade);

        // like'ın sahibi - business ilişkisi
        b.HasOne(x => x.Business)
         .WithMany() 
         .HasForeignKey(x => x.BusinessId)
         .OnDelete(DeleteBehavior.Restrict); 
    }
}
