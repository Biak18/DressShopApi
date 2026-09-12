using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public class FavoriteConfiguration
    : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        _ = builder.ToTable("favorites");

        _ = builder.HasKey(f => new
        {
            f.UserId,
            f.ProductId
        });

        _ = builder.Property(f => f.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        _ = builder.Property(f => f.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        _ = builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        _ = builder.HasOne(f => f.Product)
            .WithMany()
            .HasForeignKey(f => f.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
