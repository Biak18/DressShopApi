using DressShop.Application.Abstractions;
using DressShop.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.Categories;

public sealed class GetAdminCategoriesQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<
    GetAdminCategoriesQuery,
    IReadOnlyList<AdminCategoryDto>>
{
    public async Task<IReadOnlyList<AdminCategoryDto>> Handle(
        GetAdminCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .Select(category => new AdminCategoryDto(
                category.Id,
                category.Name,
                category.Slug,
                category.Description,
                category.IsActive,
                category.SortOrder ?? 0
            ))
            .ToListAsync(cancellationToken);
    }
}
