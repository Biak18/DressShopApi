using DressShop.Application.Abstractions;
using DressShop.Application.Features.Admin.DTOs;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.Categories;

public sealed class CreateCategoryCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<
    CreateCategoryCommand,
    AdminCategoryDto>
{
    public async Task<AdminCategoryDto> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
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
                category => category.Slug == slug,
                cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException(
                "A category with this slug already exists.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            Description = request.Request.Description,
            IsActive = request.Request.IsActive,
            SortOrder = request.Request.SortOrder
        };

        context.Categories.Add(category);

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
