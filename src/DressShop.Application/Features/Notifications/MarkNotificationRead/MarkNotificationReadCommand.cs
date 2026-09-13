using MediatR;

namespace DressShop.Application.Features.Notifications.MarkNotificationRead;

public sealed record MarkNotificationReadCommand(
    Guid UserId,
    Guid NotificationId
) : IRequest<bool>;
