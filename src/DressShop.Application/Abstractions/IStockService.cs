namespace DressShop.Application.Abstractions;

public interface IStockService
{
    Task<bool> DecrementAsync(
        Guid variantId,
        int quantity,
        CancellationToken cancellationToken);

    Task<bool> RestoreAsync(
        Guid variantId,
        int quantity,
        CancellationToken cancellationToken);
}
