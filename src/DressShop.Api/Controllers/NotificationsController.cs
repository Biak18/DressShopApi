using DressShop.Application.Abstractions;
using DressShop.Application.Features.Notifications.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController(
    INotificationService notificationService,
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

        var notifications =
            await notificationService.ListAsync(
                userId,
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

        var count =
            await notificationService.GetUnreadCountAsync(
                userId,
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

        var updated =
            await notificationService.MarkReadAsync(
                userId,
                id,
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

        await notificationService.MarkAllReadAsync(
            userId,
            cancellationToken);

        return NoContent();
    }
}
