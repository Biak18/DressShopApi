using DressShop.Application.Abstractions;
using DressShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Infrastructure.Services;

public sealed class StockService(
    AppDbContext context
) : IStockService
{
    public async Task<bool> DecrementAsync(
        Guid variantId,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        var affectedRows = await context.ProductVariants
            .Where(v =>
                v.Id == variantId &&
                v.IsActive &&
                v.StockQuantity >= quantity)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        v => v.StockQuantity,
                        v => v.StockQuantity - quantity)
                    .SetProperty(
                        v => v.UpdatedAt,
                        DateTime.UtcNow),
                cancellationToken);

        return affectedRows == 1;
    }

    public async Task<bool> RestoreAsync(
        Guid variantId,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        var affectedRows = await context.ProductVariants
            .Where(v => v.Id == variantId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        v => v.StockQuantity,
                        v => v.StockQuantity + quantity)
                    .SetProperty(
                        v => v.UpdatedAt,
                        DateTime.UtcNow),
                cancellationToken);

        return affectedRows == 1;
    }
}
