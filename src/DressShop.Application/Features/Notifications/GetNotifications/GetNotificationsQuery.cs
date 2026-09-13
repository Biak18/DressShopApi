using DressShop.Application.Features.Notifications.DTOs;
using MediatR;

namespace DressShop.Application.Features.Notifications.GetNotifications;

public sealed record GetNotificationsQuery(
    Guid UserId
) : IRequest<IReadOnlyList<NotificationDto>>;
