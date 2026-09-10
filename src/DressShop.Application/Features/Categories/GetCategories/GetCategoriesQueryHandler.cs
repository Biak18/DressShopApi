using DressShop.Application.Abstractions;
using DressShop.Application.Features.Categories.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Categories.GetCategories;

public class GetCategoriesQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
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
            .ToListAsync(cancellationToken);
    }
}
