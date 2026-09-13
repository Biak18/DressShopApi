using DressShop.Application.Abstractions;
using DressShop.Application.Features.Notifications.DTOs;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Notifications.UpdateNotificationPreferences;

public sealed class UpdateNotificationPreferencesCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<
    UpdateNotificationPreferencesCommand,
    NotificationPreferenceDto>
{
    public async Task<NotificationPreferenceDto> Handle(
        UpdateNotificationPreferencesCommand request,
        CancellationToken cancellationToken)
    {
        var preference =
            await context.NotificationPreferences
                .FirstOrDefaultAsync(
                    x => x.UserId == request.UserId,
                    cancellationToken);

        if (preference is null)
        {
            preference = new NotificationPreference
            {
                UserId = request.UserId,
                OrderUpdates =
                    request.Request.OrderUpdates ?? true,
                BackInStock =
                    request.Request.BackInStock ?? true,
                PriceDrop =
                    request.Request.PriceDrop ?? true,
                UpdatedAt = DateTime.UtcNow
            };

            context.NotificationPreferences.Add(preference);
        }
        else
        {
            if (request.Request.OrderUpdates.HasValue)
            {
                preference.OrderUpdates =
                    request.Request.OrderUpdates.Value;
            }

            if (request.Request.BackInStock.HasValue)
            {
                preference.BackInStock =
                    request.Request.BackInStock.Value;
            }

            if (request.Request.PriceDrop.HasValue)
            {
                preference.PriceDrop =
                    request.Request.PriceDrop.Value;
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
}
