using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Cart.ClearCart;

public sealed class ClearCartCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<ClearCartCommand>
{
    public async Task Handle(
        ClearCartCommand request,
        CancellationToken cancellationToken)
    {
        await context.CartItems
            .Where(x => x.UserId == request.UserId)
            .ExecuteDeleteAsync(
                cancellationToken);
    }
}
