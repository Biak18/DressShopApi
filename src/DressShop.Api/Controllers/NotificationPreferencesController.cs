using DressShop.Application.Abstractions;
using DressShop.Application.Features.Notifications.DTOs;
using DressShop.Application.Features.Notifications.GetNotificationPreferences;
using DressShop.Application.Features.Notifications.UpdateNotificationPreferences;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/notification-preferences")]
[EnableRateLimiting("api")]
[Authorize]
public sealed class NotificationPreferencesController(
    ISender sender,
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

        var preferences = await sender.Send(
            new GetNotificationPreferencesQuery(userId),
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

        var preferences = await sender.Send(
            new UpdateNotificationPreferencesCommand(
                userId,
                request),
            cancellationToken);

        return Ok(preferences);
    }
}
