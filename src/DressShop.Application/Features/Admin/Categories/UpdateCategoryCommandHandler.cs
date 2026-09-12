using DressShop.Application.Abstractions;
using DressShop.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.Categories;

public sealed class UpdateCategoryCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<
    UpdateCategoryCommand,
    AdminCategoryDto>
{
    public async Task<AdminCategoryDto> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (category is null)
        {
            throw new KeyNotFoundException(
                "Category not found.");
        }

        var name = request.Request.Name.Trim();
        var slug = request.Request.Slug.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException(
                "Category name is required.");
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new InvalidOperationException(
                "Category slug is required.");
        }

        var slugExists = await context.Categories
            .AnyAsync(
                existing =>
                    existing.Id != request.CategoryId &&
                    existing.Slug == slug,
                cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException(
                "A category with this slug already exists.");
        }

        category.Name = name;
        category.Slug = slug;
        category.Description = request.Request.Description;
        category.IsActive = request.Request.IsActive;
        category.SortOrder = request.Request.SortOrder;

        await context.SaveChangesAsync(cancellationToken);

        return new AdminCategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.IsActive,
            category.SortOrder ?? 0
        );
    }
}
