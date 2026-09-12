using DressShop.Application.Features.Admin.Categories;
using DressShop.Application.Features.Admin.DTOs;
using DressShop.Application.Features.Admin.GetLowStock;
using DressShop.Application.Features.Admin.GetOrders;
using DressShop.Application.Features.Admin.GetStats;
using DressShop.Application.Features.Admin.UpdateOrderStatus;
using DressShop.Application.Features.Admin.UpdateProductActive;
using DressShop.Application.Features.Admin.UpdateVariantStock;
using DressShop.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "Admin")]
public sealed class AdminController(
    ISender sender
) : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new
        {
            message = "Admin authorization works."
        });
    }

    [HttpGet("stats")]
    public async Task<ActionResult<AdminStatsDto>> GetStats(
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAdminStatsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("orders")]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders(
    [FromQuery] int limit = 50,
    CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetAdminOrdersQuery(limit),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IReadOnlyList<AdminLowStockItemDto>>> GetLowStock(
    [FromQuery] int threshold = 3,
    [FromQuery] int limit = 20,
    CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetAdminLowStockQuery(threshold, limit),
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("products/{id:guid}/active")]
    public async Task<IActionResult> UpdateProductActive(
    Guid id,
    UpdateProductActiveRequest request,
    CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateProductActiveCommand(
                id,
                request.IsActive),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("variants/{id:guid}/stock")]
    public async Task<IActionResult> UpdateVariantStock(
    Guid id,
    UpdateVariantStockRequest request,
    CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateVariantStockCommand(
                id,
                request.Quantity),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("orders/{id:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
    Guid id,
    UpdateOrderStatusRequest request,
    CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateOrderStatusCommand(
                id,
                request.Status),
            cancellationToken);

        return NoContent();
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<AdminCategoryDto>>> GetCategories(
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAdminCategoriesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("categories")]
    public async Task<ActionResult<AdminCategoryDto>> CreateCategory(
    CreateCategoryRequest request,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateCategoryCommand(request),
            cancellationToken);

        return Created(
            $"/api/admin/categories/{result.Id}",
            result);
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<ActionResult<AdminCategoryDto>> UpdateCategory(
    Guid id,
    UpdateCategoryRequest request,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateCategoryCommand(id, request),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(
    Guid id,
    CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteCategoryCommand(id),
            cancellationToken);

        return NoContent();
    }
}
