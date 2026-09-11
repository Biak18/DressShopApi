using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public class FavoriteConfiguration
    : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("favorites");

        builder.HasKey(f => new
        {
            f.UserId,
            f.ProductId
        });

        builder.Property(f => f.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(f => f.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");
    }
}
