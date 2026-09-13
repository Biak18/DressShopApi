using DressShop.Application.Abstractions;
using DressShop.Application.Features.Notifications.DTOs;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Notifications.GetNotificationPreferences;

public sealed class GetNotificationPreferencesQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<
    GetNotificationPreferencesQuery,
    NotificationPreferenceDto>
{
    public async Task<NotificationPreferenceDto> Handle(
        GetNotificationPreferencesQuery request,
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
}
