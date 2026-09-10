namespace DressShop.Application.Features.Products.CreateProduct;

public record ProductDto(
    Guid Id,
    string Name,
    string Slug,
    decimal BasePrice,
    Guid? CategoryId,
    string? Description,
    string? Occasion,
    string? Style,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
