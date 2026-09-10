using DressShop.Application.Features.Products.CreateProduct;
using MediatR;

namespace DressShop.Application.Features.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Slug,
    decimal BasePrice,
    Guid? CategoryId,
    string? Description,
    string? Occasion,
    string? Style
) : IRequest<ProductDto>;
