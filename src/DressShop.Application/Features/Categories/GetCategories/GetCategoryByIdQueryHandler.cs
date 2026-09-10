using DressShop.Application.Abstractions;
using DressShop.Application.Features.Categories.DTOs;
using DressShop.Application.Features.Categories.GetCategories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Categories.GetCategoryById;

public class GetCategoryByIdQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .AsNoTracking()
            .Where(c => c.Id == request.Id && c.IsActive)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.ImageUrl,
                c.SortOrder,
                c.IsActive,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return category
            ?? throw new KeyNotFoundException(
                $"Category with ID '{request.Id}' was not found.");
    }
}
