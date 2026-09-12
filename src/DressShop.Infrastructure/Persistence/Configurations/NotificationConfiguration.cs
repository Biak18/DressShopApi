using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(
        EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired();

        builder.Property(x => x.Body)
            .HasColumnName("body");

        builder.Property(x => x.Data)
            .HasColumnName("data")
            .HasColumnType("jsonb");

        builder.Property(x => x.IsRead)
            .HasColumnName("is_read")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.IsRead);

        builder.HasIndex(x => x.CreatedAt);
    }
}
