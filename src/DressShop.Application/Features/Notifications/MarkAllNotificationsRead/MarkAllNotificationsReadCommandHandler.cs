using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Notifications.MarkAllNotificationsRead;

public sealed class MarkAllNotificationsReadCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<MarkAllNotificationsReadCommand>
{
    public async Task Handle(
        MarkAllNotificationsReadCommand request,
        CancellationToken cancellationToken)
    {
        await context.Notifications
            .Where(x =>
                x.UserId == request.UserId &&
                !x.IsRead)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.IsRead,
                        true),
                cancellationToken);
    }
}
