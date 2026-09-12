using System.Security.Claims;
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
    ISender sender
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var orders = await sender.Send(
            new GetOrdersQuery(userId.Value),
            cancellationToken);

        return Ok(orders);
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue("sub");

        return Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : null;
    }
}
