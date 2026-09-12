namespace DressShop.Application.Features.Admin.DTOs;

public sealed record AdminLowStockItemDto(
    Guid Id,
    string Sku,
    string? Color,
    string? Size,
    int StockQuantity,
    AdminLowStockProductDto? Product
);

public sealed record AdminLowStockProductDto(
    Guid Id,
    string Name,
    string Slug
);
