namespace DressShop.Application.Features.Products.DTOs;

public record ProductImageDto(
    Guid Id,
    Guid ProductId,
    string ImageUrl,
    string? AltText,
    int SortOrder,
    bool IsPrimary
);
