using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Profile> Profiles { get; }

    DbSet<Category> Categories { get; }

    DbSet<Product> Products { get; }

    DbSet<ProductImage> ProductImages { get; }

    DbSet<ProductVariant> ProductVariants { get; }

    DbSet<Order> Orders { get; }

    DbSet<OrderItem> OrderItems { get; }

    DbSet<Review> Reviews { get; }

    DbSet<Favorite> Favorites { get; }

    DbSet<LoyaltyAccount> LoyaltyAccounts { get; }

    DbSet<LoyaltyTransaction> LoyaltyTransactions { get; }

    DbSet<CartItem> CartItems { get; }

    DbSet<Notification> Notifications { get; }

    DbSet<NotificationPreference> NotificationPreferences { get; }

    DbSet<Address> Addresses { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
