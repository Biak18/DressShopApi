namespace DressShop.Application.Features.Products.DTOs;

public record ProductDetailDto(
    Guid Id,
    string Name,
    string Slug,
    decimal BasePrice,
    string? Description,
    string? Style,
    string? Occasion,
    Guid? CategoryId,
    string? CategoryName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<ProductImageDto> Images,
    IReadOnlyList<ProductVariantDto> Variants
);
