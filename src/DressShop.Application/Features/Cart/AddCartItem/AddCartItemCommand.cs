using MediatR;

namespace DressShop.Application.Features.Cart.AddCartItem;

public sealed record AddCartItemCommand(
    Guid UserId,
    Guid VariantId,
    int Quantity
) : IRequest;
