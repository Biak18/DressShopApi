using DressShop.Application.Features.Cart.DTOs;
using MediatR;

namespace DressShop.Application.Features.Cart.GetCart;

public sealed record GetCartQuery(
    Guid UserId
) : IRequest<IReadOnlyList<CartItemDto>>;
