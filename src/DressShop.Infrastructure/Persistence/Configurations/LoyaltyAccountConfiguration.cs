using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public class LoyaltyAccountConfiguration
    : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        _ = builder.ToTable("loyalty_accounts");

        // Primary key
        _ = builder.HasKey(a => a.UserId);

        _ = builder.Property(a => a.UserId)
            .HasColumnName("user_id");

        // Points
        _ = builder.Property(a => a.Points)
            .HasColumnName("points")
            .IsRequired()
            .HasDefaultValue(0);

        // Tier
        _ = builder.Property(a => a.Tier)
            .HasColumnName("tier")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("bronze");

        // Referral code
        _ = builder.Property(a => a.ReferralCode)
            .HasColumnName("referral_code")
            .HasMaxLength(100);

        _ = builder.HasIndex(a => a.ReferralCode)
            .IsUnique();

        // Timestamps
        _ = builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        _ = builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Index
        _ = builder.HasIndex(a => a.Tier)
            .HasDatabaseName("loyalty_accounts_tier_idx");
    }
}
