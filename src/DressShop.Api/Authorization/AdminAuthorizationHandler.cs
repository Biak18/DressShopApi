using DressShop.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Api.Authorization;

public sealed class AdminAuthorizationHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser
) : AuthorizationHandler<AdminRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext authorizationContext,
        AdminRequirement requirement)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return;
        }

        var isAdmin = await context.Profiles
            .AsNoTracking()
            .AnyAsync(
                profile =>
                    profile.Id == userId &&
                    profile.Role == "admin");

        if (isAdmin)
        {
            authorizationContext.Succeed(requirement);
        }
    }
}
