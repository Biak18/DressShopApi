using System.Data;
using DressShop.Application.Abstractions;
using DressShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Infrastructure.Services;

public sealed class CartItemWriter(
    AppDbContext context
) : ICartItemWriter
{
    public async Task AddAsync(
        Guid userId,
        Guid variantId,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be at least 1");
        }

        var connection = context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();

        command.CommandText = """
            select 1
            from public.add_to_cart_for_user(
                @p_user_id,
                @p_variant_id,
                @p_quantity
            );
            """;

        var userIdParameter = command.CreateParameter();
        userIdParameter.ParameterName = "p_user_id";
        userIdParameter.Value = userId;
        command.Parameters.Add(userIdParameter);

        var variantIdParameter = command.CreateParameter();
        variantIdParameter.ParameterName = "p_variant_id";
        variantIdParameter.Value = variantId;
        command.Parameters.Add(variantIdParameter);

        var quantityParameter = command.CreateParameter();
        quantityParameter.ParameterName = "p_quantity";
        quantityParameter.Value = quantity;
        command.Parameters.Add(quantityParameter);

        await command.ExecuteScalarAsync(cancellationToken);
    }
}
