using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        _ = builder.ToTable("orders");

        // Primary key
        _ = builder.HasKey(o => o.Id);

        _ = builder.Property(o => o.Id)
            .HasColumnName("id");

        // User
        _ = builder.Property(o => o.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // Status
        _ = builder.Property(o => o.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("pending");

        // Subtotal
        _ = builder.Property(o => o.Subtotal)
            .HasColumnName("subtotal")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        // Shipping amount
        _ = builder.Property(o => o.ShippingAmount)
            .HasColumnName("shipping_amount")
            .HasColumnType("numeric(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        // Discount amount
        _ = builder.Property(o => o.DiscountAmount)
            .HasColumnName("discount_amount")
            .HasColumnType("numeric(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        // Total
        _ = builder.Property(o => o.Total)
            .HasColumnName("total")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        // Shipping address
        _ = builder.Property(o => o.ShippingAddress)
            .HasColumnName("shipping_address")
            .HasColumnType("jsonb");

        // Created timestamp
        _ = builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Updated timestamp
        _ = builder.Property(o => o.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // User relationship
        // We intentionally don't add a Profile navigation here.
        // user_id references profiles(id).

        // Order -> OrderItems
        _ = builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        _ = builder.HasIndex(o => o.UserId)
            .HasDatabaseName("orders_user_id_idx");

        _ = builder.HasIndex(o => o.Status)
            .HasDatabaseName("orders_status_idx");

        _ = builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("orders_created_at_idx")
            .IsDescending();
    }
}
