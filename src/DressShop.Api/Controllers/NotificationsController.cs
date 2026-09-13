using DressShop.Application.Abstractions;
using DressShop.Application.Features.Notifications.DTOs;
using DressShop.Application.Features.Notifications.GetNotifications;
using DressShop.Application.Features.Notifications.GetUnreadCount;
using DressShop.Application.Features.Notifications.MarkAllNotificationsRead;
using DressShop.Application.Features.Notifications.MarkNotificationRead;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController(
    ISender sender,
    ICurrentUser currentUser
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> List(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var notifications = await sender.Send(
            new GetNotificationsQuery(userId),
            cancellationToken);

        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> UnreadCount(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var count = await sender.Send(
            new GetUnreadCountQuery(userId),
            cancellationToken);

        return Ok(count);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var updated = await sender.Send(
            new MarkNotificationReadCommand(
                userId,
                id),
            cancellationToken);

        return updated
            ? NoContent()
            : NotFound();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        await sender.Send(
            new MarkAllNotificationsReadCommand(userId),
            cancellationToken);

        return NoContent();
    }
}
