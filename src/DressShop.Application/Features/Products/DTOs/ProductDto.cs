using DressShop.Application.Features.Categories.DTOs;

namespace DressShop.Application.Features.Products.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Slug,
    decimal BasePrice,
    Guid? CategoryId,
    CategoryDto? Category,
    string? Description,
    string? Style,
    string? Occasion,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<ProductImageDto> Images,
    IReadOnlyList<ProductVariantDto> Variants
);
