using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Notifications.MarkNotificationRead;

public sealed class MarkNotificationReadCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<
    MarkNotificationReadCommand,
    bool>
{
    public async Task<bool> Handle(
        MarkNotificationReadCommand request,
        CancellationToken cancellationToken)
    {
        var affectedRows = await context.Notifications
            .Where(x =>
                x.Id == request.NotificationId &&
                x.UserId == request.UserId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.IsRead,
                        true),
                cancellationToken);

        return affectedRows == 1;
    }
}
