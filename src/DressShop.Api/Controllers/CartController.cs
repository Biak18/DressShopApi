using DressShop.Application.Abstractions;
using DressShop.Application.Features.Cart.AddCartItem;
using DressShop.Application.Features.Cart.ClearCart;
using DressShop.Application.Features.Cart.DTOs;
using DressShop.Application.Features.Cart.GetCart;
using DressShop.Application.Features.Cart.RemoveCartItem;
using DressShop.Application.Features.Cart.UpdateCartItem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/cart")]
[EnableRateLimiting("api")]
[Authorize]
public sealed class CartController(
    ISender sender,
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

        var cart = await sender.Send(
            new GetCartQuery(userId),
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

        await sender.Send(
            new AddCartItemCommand(
                userId,
                request.VariantId,
                request.Quantity),
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

        await sender.Send(
            new UpdateCartItemCommand(
                userId,
                id,
                request.Quantity),
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

        await sender.Send(
            new RemoveCartItemCommand(
                userId,
                id),
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

        await sender.Send(
            new ClearCartCommand(userId),
            cancellationToken);

        return NoContent();
    }
}
