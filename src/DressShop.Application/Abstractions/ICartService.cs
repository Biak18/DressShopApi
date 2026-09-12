namespace DressShop.Application.Abstractions;

public interface ICartService
{
    Task RemoveItemsAsync(
        Guid userId,
        IReadOnlyList<Guid> variantIds,
        CancellationToken cancellationToken);
}
