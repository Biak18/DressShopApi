using DressShop.Application.Features.Products.DTOs;
using MediatR;

namespace DressShop.Application.Features.Products.GetProducts;

public record GetProductsQuery(
    int Page = 0,
    int PageSize = 10,
    bool IsActive = true,
    Guid? CategoryId = null,
    string? CategorySlug = null,
    string? Search = null,
    string? Style = null,
    string? Occasion = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? Color = null,
    string? Size = null,
    bool? InStock = null,
    string? Sort = null
) : IRequest<PaginatedProductsDto>;
