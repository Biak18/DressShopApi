namespace DressShop.Application.Features.Products.DTOs;

public record PaginatedProductsDto(
    IReadOnlyList<ProductDto> Data,
    int Count,
    int Page,
    int PageSize
);
