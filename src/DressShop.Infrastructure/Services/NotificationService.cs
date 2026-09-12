using System.Text.Json;
using DressShop.Application.Abstractions;
using DressShop.Domain.Entities;
using DressShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Infrastructure.Services;

public sealed class NotificationService(
    AppDbContext context
) : INotificationService
{
    public async Task CreateOrderConfirmedAsync(
        Guid userId,
        Guid orderId,
        decimal total,
        CancellationToken cancellationToken)
    {
        var preference =
            await context.NotificationPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);

        // No preference row means notifications are enabled.
        if (preference is not null &&
            !preference.OrderUpdates)
        {
            return;
        }

        var orderNumber =
            orderId
                .ToString()[..8]
                .ToUpperInvariant();

        var data = JsonSerializer.Serialize(
            new
            {
                order_id = orderId,
                total
            });

        context.Notifications.Add(
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = "order_confirmed",
                Title = "Order confirmed",
                Body =
                    $"Your order #{orderNumber} is confirmed. " +
                    "We'll notify when it ships.",
                Data = data,
                CreatedAt = DateTime.UtcNow
            });
    }

    public async Task CreateOrderCancelledAsync(
    Guid userId,
    Guid orderId,
    CancellationToken cancellationToken)
    {
        var preference =
            await context.NotificationPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);

        if (preference is not null &&
            !preference.OrderUpdates)
        {
            return;
        }

        var orderNumber =
            orderId
                .ToString()[..8]
                .ToUpperInvariant();

        var data = JsonSerializer.Serialize(
            new
            {
                order_id = orderId
            });

        context.Notifications.Add(
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = "general",
                Title = "Order cancelled",
                Body =
                    $"Your order #{orderNumber} has been cancelled.",
                Data = data,
                CreatedAt = DateTime.UtcNow
            });
    }
}
