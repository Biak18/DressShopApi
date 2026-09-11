using System.Security.Claims;
using DressShop.Application.Features.Favorites.CreateFavorite;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FavoritesController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateFavorite(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }


        var result = await sender.Send(new CreateFavoriteCommand(userId.Value, productId), cancellationToken);

        return Created($"api/favorites/{result.Id}", result);
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue("sub");

        return Guid.TryParse(userId, out var id)
            ? id
            : null;
    }
}
