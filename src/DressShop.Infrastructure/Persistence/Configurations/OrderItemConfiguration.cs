using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration
    : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        _ = builder.ToTable("order_items");

        // Primary key
        _ = builder.HasKey(i => i.Id);

        _ = builder.Property(i => i.Id)
            .HasColumnName("id");

        // Order
        _ = builder.Property(i => i.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        _ = builder.HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product
        _ = builder.Property(i => i.ProductId)
            .HasColumnName("product_id");

        _ = builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        // Variant
        _ = builder.Property(i => i.VariantId)
            .HasColumnName("variant_id");

        _ = builder.HasOne<ProductVariant>()
            .WithMany()
            .HasForeignKey(i => i.VariantId)
            .OnDelete(DeleteBehavior.SetNull);

        // Product name snapshot
        _ = builder.Property(i => i.ProductName)
            .HasColumnName("product_name")
            .IsRequired()
            .HasMaxLength(500);

        // Variant description snapshot
        _ = builder.Property(i => i.VariantDescription)
            .HasColumnName("variant_description")
            .HasMaxLength(500);

        // Unit price snapshot
        _ = builder.Property(i => i.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        // Quantity
        _ = builder.Property(i => i.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        // Created timestamp
        _ = builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Indexes
        _ = builder.HasIndex(i => i.OrderId)
            .HasDatabaseName("order_items_order_id_idx");

        _ = builder.HasIndex(i => i.ProductId)
            .HasDatabaseName("order_items_product_id_idx");

        _ = builder.HasIndex(i => i.VariantId)
            .HasDatabaseName("order_items_variant_id_idx");
    }
}
