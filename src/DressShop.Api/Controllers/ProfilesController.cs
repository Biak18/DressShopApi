using DressShop.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/profiles")]
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

        return profile is null
            ? NotFound()
            : Ok(profile);
    }
}
