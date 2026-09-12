using System.Security.Claims;
using DressShop.Application.Features.Favorites.CreateFavorite;
using DressShop.Application.Features.Favorites.DeleteFavorite;
using DressShop.Application.Features.Favorites.GetFavoriteIds;
using DressShop.Application.Features.Favorites.GetFavoriteStatus;
using DressShop.Application.Features.Favorites.GetWishlist;
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

    [Authorize]
    [HttpGet("ids")]
    public async Task<IActionResult> GetFavoriteIds(
    CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new GetFavoriteIdsQuery(userId.Value),
            cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetFavoriteStatus(
    Guid productId,
    CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var isFavorite = await sender.Send(
            new GetFavoriteStatusQuery(
                userId.Value,
                productId),
            cancellationToken);

        return Ok(new
        {
            isFavorite
        });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetWishlist(
    CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new GetWishlistQuery(userId.Value),
            cancellationToken);

        return Ok(result);
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
