using System.Security.Claims;
using DressShop.Application.Features.Favorites.CreateFavorite;
using DressShop.Application.Features.Favorites.DeleteFavorite;
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
      [FromBody] CreateFavoriteRequest request,
      CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new CreateFavoriteCommand(
                userId.Value,
                request.ProductId),
            cancellationToken);

        return Created(
          $"/api/favorites/{result.ProductId}",
          result);
    }

    [Authorize]
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeleteFavorite(
    Guid productId,
    CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        await sender.Send(
            new DeleteFavoriteCommand(
                userId.Value,
                productId),
            cancellationToken);

        return NoContent();
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue("sub");

        return Guid.TryParse(userId, out var id)
            ? id
            : null;
    }
}
public record CreateFavoriteRequest(Guid ProductId);
