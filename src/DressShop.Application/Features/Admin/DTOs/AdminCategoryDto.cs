namespace DressShop.Application.Features.Admin.DTOs;

public sealed record AdminCategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    bool IsActive,
    int SortOrder
);
