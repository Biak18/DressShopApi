using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public class LoyaltyTransactionConfiguration
    : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        _ = builder.ToTable("loyalty_transactions");

        // Primary key
        _ = builder.HasKey(t => t.Id);

        _ = builder.Property(t => t.Id)
            .HasColumnName("id");

        // User
        _ = builder.Property(t => t.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // Points
        _ = builder.Property(t => t.Points)
            .HasColumnName("points")
            .IsRequired();

        // Transaction type
        _ = builder.Property(t => t.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasMaxLength(50);

        // Description
        _ = builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        // Order
        _ = builder.Property(t => t.OrderId)
            .HasColumnName("order_id");

        _ = builder.HasOne(t => t.Order)
            .WithMany()
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        // Created timestamp
        _ = builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Indexes
        _ = builder.HasIndex(t => t.UserId)
            .HasDatabaseName("loyalty_tx_user_idx");

        _ = builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("loyalty_tx_created_idx")
            .IsDescending();

        _ = builder.HasIndex(t => t.OrderId)
            .HasDatabaseName("loyalty_transactions_order_id_idx");
    }
}
