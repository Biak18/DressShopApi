using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        // Primary key
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id");

        // Name
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        // Slug
        builder.Property(p => p.Slug)
            .HasColumnName("slug")
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        // Base price
        builder.Property(p => p.BasePrice)
            .HasColumnName("base_price")
            .HasColumnType("numeric(12,2)")
            .IsRequired();

        // Category
        builder.Property(p => p.CategoryId)
            .HasColumnName("category_id");

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Description
        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        // Occasion
        builder.Property(p => p.Occasion)
            .HasColumnName("occasion")
            .HasMaxLength(100);

        // Style
        builder.Property(p => p.Style)
            .HasColumnName("style")
            .HasMaxLength(100);

        // Active status
        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        // Created timestamp
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Updated timestamp
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Category
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        // Images
        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        // Variants
        builder.HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        // Reviews
        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
