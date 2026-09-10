namespace DressShop.Api.Controllers.Requests;

public record GetProductsRequest(
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
);
