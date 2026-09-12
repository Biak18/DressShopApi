namespace DressShop.Application.Features.Admin.DTOs;

public sealed record CreateCategoryRequest(
    string Name,
    string Slug,
    string? Description = null,
    bool IsActive = true,
    int SortOrder = 0
);
