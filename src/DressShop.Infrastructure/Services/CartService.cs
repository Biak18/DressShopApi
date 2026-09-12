using DressShop.Application.Abstractions;
using DressShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Infrastructure.Services;

public sealed class CartService(
    AppDbContext context
) : ICartService
{
    public async Task RemoveItemsAsync(
        Guid userId,
        IReadOnlyList<Guid> variantIds,
        CancellationToken cancellationToken)
    {
        if (variantIds.Count == 0)
        {
            return;
        }

        await context.CartItems
            .Where(item =>
                item.UserId == userId &&
                variantIds.Contains(item.VariantId))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
