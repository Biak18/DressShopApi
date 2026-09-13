using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Cart.RemoveCartItem;

public sealed class RemoveCartItemCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<RemoveCartItemCommand>
{
    public async Task Handle(
        RemoveCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var affectedRows = await context.CartItems
            .Where(x =>
                x.Id == request.CartItemId &&
                x.UserId == request.UserId)
            .ExecuteDeleteAsync(
                cancellationToken);

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException(
                "Cart item not found.");
        }
    }
}
