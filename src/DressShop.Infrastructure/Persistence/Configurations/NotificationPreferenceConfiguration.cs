using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public sealed class NotificationPreferenceConfiguration
    : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(
        EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.OrderUpdates)
            .HasColumnName("order_updates")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.Promotions)
            .HasColumnName("promotions")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.PriceDrops)
            .HasColumnName("price_drops")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.BackInStock)
            .HasColumnName("back_in_stock")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("now()");
    }
}
