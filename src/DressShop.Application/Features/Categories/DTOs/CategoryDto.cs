namespace DressShop.Application.Features.Categories.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    int? SortOrder,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
