using DressShop.Application.Abstractions;
using DressShop.Application.Features.Orders.DTOs;
using DressShop.Application.Features.Orders.GetOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(
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
}
