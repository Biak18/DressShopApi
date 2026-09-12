using DressShop.Application.Abstractions;
using DressShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

        var connection = context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();

        command.CommandText =
            "select public.restore_variant_stock(@variant_uuid, @delta);";

        var variantParameter =
            new NpgsqlParameter("variant_uuid", variantId);

        var deltaParameter =
            new NpgsqlParameter("delta", quantity);

        command.Parameters.Add(variantParameter);
        command.Parameters.Add(deltaParameter);

        await command.ExecuteNonQueryAsync(
            cancellationToken);

        return true;
    }
}
