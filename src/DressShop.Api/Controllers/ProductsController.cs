using DressShop.Application.Features.Products.CreateProduct;
using DressShop.Application.Features.Products.DeleteProduct;
using DressShop.Application.Features.Products.DTOs;
using DressShop.Application.Features.Products.GetProducts;
using DressShop.Application.Features.Products.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("api")]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetProductBySlug(
    string slug,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProductBySlugQuery(slug),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await sender.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Slug,
            request.BasePrice,
            request.CategoryId,
            request.Description,
            request.Occasion,
            request.Style
        );

        var product = await sender.Send(
            command,
            cancellationToken);

        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteProductCommand(id),
            cancellationToken);

        return NoContent();
    }
}
