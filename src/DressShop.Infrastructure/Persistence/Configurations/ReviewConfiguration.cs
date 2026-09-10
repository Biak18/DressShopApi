using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews", t => t.HasCheckConstraint("ck_reviews_rating",
            "\"rating\" >= 1 AND \"rating\" <= 5"));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id");

        builder.Property(r => r.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(r => r.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(r => r.OrderItemId)
            .HasColumnName("order_item_id");

        builder.Property(r => r.Rating)
            .HasColumnName("rating")
            .IsRequired();

        builder.Property(r => r.Body)
            .HasColumnName("body");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
