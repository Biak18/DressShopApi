using MediatR;

namespace DressShop.Application.Features.Cart.UpdateCartItem;

public sealed record UpdateCartItemCommand(
    Guid UserId,
    Guid CartItemId,
    int Quantity
) : IRequest;
