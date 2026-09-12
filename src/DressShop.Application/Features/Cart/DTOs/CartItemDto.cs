namespace DressShop.Application.Features.Cart.DTOs;

public record CartItemDto(
    Guid Id,
    int Quantity,
    Guid VariantId,
    CartVariantDto Variant,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CartVariantDto(
    Guid Id,
    Guid ProductId,
    string Sku,
    string? Color,
    string? Size,
    decimal? Price,
    int StockQuantity,
    bool IsActive,
    CartProductDto? Product
);

public record CartProductDto(
    Guid Id,
    string Name,
    decimal BasePrice,
    CartCategoryDto? Category,
    IReadOnlyList<CartProductImageDto> Images
);

public record CartCategoryDto(
    Guid Id,
    string Name
);

public record CartProductImageDto(
    Guid Id,
    string Url,
    bool IsPrimary,
    int SortOrder
);
