using DressShop.Application.Features.Categories.CreateCategory;
using DressShop.Application.Features.Categories.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ISender sender) : ControllerBase
{

    /// <summary>
    /// Get all categories
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
       CancellationToken cancellationToken)
    {
        var categories = await sender.Send(
            new GetCategoriesQuery(),
            cancellationToken);

        return Ok(categories);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var categoryId = await sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = categoryId },
            new { Id = categoryId });
    }

    // Temporary placeholder so CreatedAtAction works.
    // We will implement this properly later.
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await sender.Send(
            new GetCategoryByIdQuery(id),
            cancellationToken);

        return Ok(category);
    }
}
