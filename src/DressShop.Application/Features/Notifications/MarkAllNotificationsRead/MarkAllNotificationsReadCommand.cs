using MediatR;

namespace DressShop.Application.Features.Notifications.MarkAllNotificationsRead;

public sealed record MarkAllNotificationsReadCommand(
    Guid UserId
) : IRequest;
