using DressShop.Application.Abstractions;
using DressShop.Application.Features.Notifications.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Notifications.GetNotifications;

public sealed class GetNotificationsQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<
    GetNotificationsQuery,
    IReadOnlyList<NotificationDto>>
{
    public async Task<IReadOnlyList<NotificationDto>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
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
}
