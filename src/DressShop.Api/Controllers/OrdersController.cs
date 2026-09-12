using DressShop.Application.Abstractions;
using DressShop.Application.Features.Orders.CreateOrder;
using DressShop.Application.Features.Orders.DTOs;
using DressShop.Application.Features.Orders.GetOrder;
using DressShop.Application.Features.Orders.GetOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class OrdersController(
    ISender sender,
    ICurrentUser currentUser
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var orders = await sender.Send(
            new GetOrdersQuery(userId),
            cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var order = await sender.Send(
            new GetOrderQuery(userId, id),
            cancellationToken);

        return order is null
            ? NotFound()
            : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var order = await sender.Send(
            new CreateOrderCommand(
                userId,
                request),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetOrder),
            new { id = order.Id },
            order);
    }
}
