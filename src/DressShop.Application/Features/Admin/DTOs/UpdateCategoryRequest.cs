namespace DressShop.Application.Features.Admin.DTOs;

public sealed record UpdateCategoryRequest(
    string Name,
    string Slug,
    string? Description,
    bool IsActive,
    int SortOrder
);
