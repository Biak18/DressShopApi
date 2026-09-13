using MediatR;

namespace DressShop.Application.Features.Cart.RemoveCartItem;

public sealed record RemoveCartItemCommand(
    Guid UserId,
    Guid CartItemId
) : IRequest;
