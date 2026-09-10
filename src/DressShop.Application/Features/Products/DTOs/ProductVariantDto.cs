namespace DressShop.Application.Features.Products.DTOs;

public record ProductVariantDto(
    Guid Id,
    Guid ProductId,
    string Sku,
    string? Color,
    string? Size,
    decimal? Price,
    int StockQuantity,
    bool IsActive
);
