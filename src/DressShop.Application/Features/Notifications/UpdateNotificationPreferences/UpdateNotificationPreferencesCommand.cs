using DressShop.Application.Features.Notifications.DTOs;
using MediatR;

namespace DressShop.Application.Features.Notifications.UpdateNotificationPreferences;

public sealed record UpdateNotificationPreferencesCommand(
    Guid UserId,
    UpdateNotificationPreferencesRequest Request
) : IRequest<NotificationPreferenceDto>;
