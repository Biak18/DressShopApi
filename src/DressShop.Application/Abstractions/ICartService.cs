using DressShop.Application.Features.Cart.DTOs;

namespace DressShop.Application.Abstractions;

public interface ICartService
{
    Task<IReadOnlyList<CartItemDto>> GetCartAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task AddToCartAsync(
        Guid userId,
        Guid variantId,
        int quantity,
        CancellationToken cancellationToken);

    Task UpdateQuantityAsync(
        Guid userId,
        Guid cartItemId,
        int quantity,
        CancellationToken cancellationToken);

    Task RemoveFromCartAsync(
        Guid userId,
        Guid cartItemId,
        CancellationToken cancellationToken);

    Task ClearCartAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task RemoveItemsAsync(
        Guid userId,
        IReadOnlyList<Guid> variantIds,
        CancellationToken cancellationToken);
}
