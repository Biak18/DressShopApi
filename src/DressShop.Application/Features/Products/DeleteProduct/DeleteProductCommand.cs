using MediatR;

namespace DressShop.Application.Features.Products.DeleteProduct;

public record DeleteProductCommand(
    Guid Id
) : IRequest;
