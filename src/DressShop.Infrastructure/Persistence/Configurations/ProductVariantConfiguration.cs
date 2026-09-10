using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration
    : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasColumnName("id");

        builder.Property(v => v.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(v => v.Sku)
            .HasColumnName("sku")
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(v => v.Sku)
            .IsUnique();

        builder.Property(v => v.Color)
            .HasColumnName("color")
            .HasMaxLength(100);

        builder.Property(v => v.Size)
            .HasColumnName("size")
            .HasMaxLength(50);

        builder.Property(v => v.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(12,2)");

        builder.Property(v => v.StockQuantity)
            .HasColumnName("stock_quantity")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(v => v.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
