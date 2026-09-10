using MediatR;

namespace DressShop.Application.Features.Categories.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    int? SortOrder
) : IRequest<Guid>;   // Returns the new Category Id
