using System.Text.Json;
using DressShop.Application.Abstractions;
using DressShop.Application.Features.Notifications.DTOs;
using DressShop.Domain.Entities;
using DressShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Infrastructure.Services;

public sealed class NotificationService(
    AppDbContext context
) : INotificationService
{
    public async Task<IReadOnlyList<NotificationDto>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(50)
            .Select(x => new NotificationDto(
                x.Id,
                x.Type,
                x.Title,
                x.Body,
                x.Data,
                x.IsRead,
                x.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await context.Notifications
            .CountAsync(
                x =>
                    x.UserId == userId &&
                    !x.IsRead,
                cancellationToken);
    }

    public async Task<bool> MarkReadAsync(
        Guid userId,
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        var affectedRows = await context.Notifications
            .Where(x =>
                x.Id == notificationId &&
                x.UserId == userId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.IsRead,
                        true),
                cancellationToken);

        return affectedRows == 1;
    }

    public async Task MarkAllReadAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        await context.Notifications
            .Where(x =>
                x.UserId == userId &&
                !x.IsRead)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.IsRead,
                        true),
                cancellationToken);
    }

    public async Task<NotificationPreferenceDto> GetPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var preference =
            await context.NotificationPreferences
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);

        if (preference is null)
        {
            preference = new NotificationPreference
            {
                UserId = userId,
                OrderUpdates = true,
                BackInStock = true,
                PriceDrop = true,
                UpdatedAt = DateTime.UtcNow
            };

            context.NotificationPreferences.Add(preference);

            await context.SaveChangesAsync(
                cancellationToken);
        }

        return new NotificationPreferenceDto(
            preference.OrderUpdates,
            preference.BackInStock,
            preference.PriceDrop,
            preference.UpdatedAt
        );
    }

    public async Task<NotificationPreferenceDto> UpdatePreferencesAsync(
        Guid userId,
        UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var preference =
            await context.NotificationPreferences
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);

        if (preference is null)
        {
            preference = new NotificationPreference
            {
                UserId = userId,
                OrderUpdates = request.OrderUpdates ?? true,
                BackInStock = request.BackInStock ?? true,
                PriceDrop = request.PriceDrop ?? true,
                UpdatedAt = DateTime.UtcNow
            };

            context.NotificationPreferences.Add(preference);
        }
        else
        {
            if (request.OrderUpdates.HasValue)
            {
                preference.OrderUpdates =
                    request.OrderUpdates.Value;
            }

            if (request.BackInStock.HasValue)
            {
                preference.BackInStock =
                    request.BackInStock.Value;
            }

            if (request.PriceDrop.HasValue)
            {
                preference.PriceDrop =
                    request.PriceDrop.Value;
            }

            preference.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(
            cancellationToken);

        return new NotificationPreferenceDto(
            preference.OrderUpdates,
            preference.BackInStock,
            preference.PriceDrop,
            preference.UpdatedAt
        );
    }

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
