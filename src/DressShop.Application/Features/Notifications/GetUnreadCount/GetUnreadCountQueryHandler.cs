using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Notifications.GetUnreadCount;

public sealed class GetUnreadCountQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetUnreadCountQuery, int>
{
    public Task<int> Handle(
        GetUnreadCountQuery request,
        CancellationToken cancellationToken)
    {
        return context.Notifications
            .CountAsync(
                x =>
                    x.UserId == request.UserId &&
                    !x.IsRead,
                cancellationToken);
    }
}
