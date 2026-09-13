using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Cart.UpdateCartItem;

public sealed class UpdateCartItemCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<UpdateCartItemCommand>
{
    public async Task Handle(
        UpdateCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var cartItem = await context.CartItems
            .Include(x => x.Variant)
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.CartItemId &&
                    x.UserId == request.UserId,
                cancellationToken);

        if (cartItem is null)
        {
            throw new KeyNotFoundException(
                "Cart item not found.");
        }

        if (request.Quantity <= 0)
        {
            context.CartItems.Remove(cartItem);

            await context.SaveChangesAsync(
                cancellationToken);

            return;
        }

        if (!cartItem.Variant.IsActive)
        {
            throw new InvalidOperationException(
                "Product variant is unavailable.");
        }

        if (request.Quantity > cartItem.Variant.StockQuantity)
        {
            throw new InvalidOperationException(
                $"Only {cartItem.Variant.StockQuantity} in stock.");
        }

        cartItem.Quantity = request.Quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(
            cancellationToken);
    }
}
