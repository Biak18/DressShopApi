using MediatR;

namespace DressShop.Application.Features.Cart.ClearCart;

public sealed record ClearCartCommand(
    Guid UserId
) : IRequest;
