using DressShop.Application.Abstractions;
using DressShop.Application.Features.Favorites.CreateFavorite;
using DressShop.Application.Features.Favorites.DeleteFavorite;
using DressShop.Application.Features.Favorites.GetFavoriteIds;
using DressShop.Application.Features.Favorites.GetFavoriteStatus;
using DressShop.Application.Features.Favorites.GetWishlist;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DressShop.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("api")]
public class FavoritesController(ISender sender, ICurrentUser currentUser) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateFavorite(
      [FromBody] CreateFavoriteRequest request,
      CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new CreateFavoriteCommand(
                userId,
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
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        await sender.Send(
            new DeleteFavoriteCommand(
                userId,
                productId),
            cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpGet("ids")]
    public async Task<IActionResult> GetFavoriteIds(
    CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new GetFavoriteIdsQuery(userId),
            cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetFavoriteStatus(
    Guid productId,
    CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var isFavorite = await sender.Send(
            new GetFavoriteStatusQuery(
                userId,
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
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new GetWishlistQuery(userId),
            cancellationToken);

        return Ok(result);
    }
}
public record CreateFavoriteRequest(Guid ProductId);
