namespace DressShop.Application.Abstractions;

public interface ICartItemWriter
{
    Task AddAsync(
        Guid userId,
        Guid variantId,
        int quantity,
        CancellationToken cancellationToken);
}
