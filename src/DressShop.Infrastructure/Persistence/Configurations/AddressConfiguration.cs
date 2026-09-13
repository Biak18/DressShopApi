using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DressShop.Infrastructure.Persistence.Configurations;

public sealed class AddressConfiguration
    : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");

        builder.HasKey(address => address.Id);

        builder.Property(address => address.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(address => address.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(address => address.Label)
            .HasColumnName("label");

        builder.Property(address => address.RecipientName)
            .HasColumnName("recipient_name")
            .IsRequired();

        builder.Property(address => address.Phone)
            .HasColumnName("phone");

        builder.Property(address => address.AddressLine1)
            .HasColumnName("address_line_1")
            .IsRequired();

        builder.Property(address => address.AddressLine2)
            .HasColumnName("address_line_2");

        builder.Property(address => address.City)
            .HasColumnName("city")
            .IsRequired();

        builder.Property(address => address.State)
            .HasColumnName("state");

        builder.Property(address => address.PostalCode)
            .HasColumnName("postal_code");

        builder.Property(address => address.Country)
            .HasColumnName("country")
            .IsRequired()
            .HasDefaultValue("US");

        builder.Property(address => address.IsDefault)
            .HasColumnName("is_default")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(address => address.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(address => address.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasIndex(address => address.UserId)
            .HasDatabaseName("addresses_user_id_idx");

        builder.HasOne<Profile>()
            .WithMany()
            .HasForeignKey(address => address.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
