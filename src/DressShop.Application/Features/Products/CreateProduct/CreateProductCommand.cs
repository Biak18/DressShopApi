using MediatR;

namespace DressShop.Application.Features.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Slug,
    decimal BasePrice,
    Guid? CategoryId,
    string? Description,
    string? Occasion,
    string? Style
) : IRequest<ProductDto>;
