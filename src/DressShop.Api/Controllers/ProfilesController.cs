using System.Net.Http.Headers;
using System.Security.Claims;
using DressShop.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("profiles")]
[Authorize]
public sealed class ProfilesController(IProfilesClient profilesClient) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authorization)
            || !string.Equals(authorization.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(authorization.Parameter))
        {
            return Unauthorized();
        }

        var profile = await profilesClient.GetCurrentAsync(
            userId,
            authorization.Parameter,
            cancellationToken);

        return profile is null ? NotFound() : Ok(profile.Value);
    }
}
