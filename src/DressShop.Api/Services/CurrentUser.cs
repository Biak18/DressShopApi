using System.Security.Claims;
using DressShop.Application.Abstractions;

namespace DressShop.Api.Services;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor
) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var value = httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue("sub");

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }

    public bool IsAuthenticated =>
        httpContextAccessor
            .HttpContext?
            .User?
            .Identity?
            .IsAuthenticated == true;
}
