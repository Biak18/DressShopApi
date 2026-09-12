using DressShop.Application.Abstractions;
using DressShop.Application.Features.Notifications.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/notification-preferences")]
[Authorize]
public sealed class NotificationPreferencesController(
    INotificationService notificationService,
    ICurrentUser currentUser
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<NotificationPreferenceDto>> Get(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var preferences =
            await notificationService.GetPreferencesAsync(
                userId,
                cancellationToken);

        return Ok(preferences);
    }

    [HttpPut]
    public async Task<ActionResult<NotificationPreferenceDto>> Update(
        UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var preferences =
            await notificationService.UpdatePreferencesAsync(
                userId,
                request,
                cancellationToken);

        return Ok(preferences);
    }
}
