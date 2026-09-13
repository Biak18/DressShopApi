using DressShop.Application.Features.Notifications.DTOs;
using MediatR;

namespace DressShop.Application.Features.Notifications.GetNotificationPreferences;

public sealed record GetNotificationPreferencesQuery(
    Guid UserId
) : IRequest<NotificationPreferenceDto>;
