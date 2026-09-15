using DressShop.Application.Abstractions;
using DressShop.Application.Features.Profiles.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/profiles")]
[EnableRateLimiting("api")]
[Authorize]
public sealed class ProfilesController(
    IApplicationDbContext context,
    ICurrentUser currentUser
) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrent(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var profile = await context.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.Id == userId,
                cancellationToken);

        if (profile is null)
        {
            return NotFound();
        }

        return Ok(new ProfileDto(
            profile.Id,
            profile.FullName,
            profile.AvatarUrl,
            profile.Role,
            currentUser.Email,
            profile.CreatedAt,
            profile.UpdatedAt));
    }
}
