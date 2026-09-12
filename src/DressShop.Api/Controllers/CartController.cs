using DressShop.Application.Abstractions;
using DressShop.Application.Features.Cart.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public sealed class CartController(
    ICartService cartService,
    ICurrentUser currentUser
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CartItemDto>>> GetCart(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var cart = await cartService.GetCartAsync(
            userId,
            cancellationToken);

        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart(
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        await cartService.AddToCartAsync(
            userId,
            request.VariantId,
            request.Quantity,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("items/{id:guid}")]
    public async Task<IActionResult> UpdateQuantity(
        Guid id,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        await cartService.UpdateQuantityAsync(
            userId,
            id,
            request.Quantity,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("items/{id:guid}")]
    public async Task<IActionResult> RemoveFromCart(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        await cartService.RemoveFromCartAsync(
            userId,
            id,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        await cartService.ClearCartAsync(
            userId,
            cancellationToken);

        return NoContent();
    }
}
