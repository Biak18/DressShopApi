using MediatR;

namespace DressShop.Application.Features.Notifications.GetUnreadCount;

public sealed record GetUnreadCountQuery(
    Guid UserId
) : IRequest<int>;
