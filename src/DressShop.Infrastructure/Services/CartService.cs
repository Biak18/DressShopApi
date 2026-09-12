using DressShop.Application.Abstractions;
using DressShop.Application.Features.Cart.DTOs;
using DressShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Infrastructure.Services;

public sealed class CartService(
    AppDbContext context
) : ICartService
{
    public async Task<IReadOnlyList<CartItemDto>> GetCartAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var items = await context.CartItems
            .AsNoTracking()
            .Include(x => x.Variant)
                .ThenInclude(x => x.Product)
            .Include(x => x.Variant)
                .ThenInclude(x => x.Product.Category)
            .Include(x => x.Variant)
                .ThenInclude(x => x.Product.Images)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return items
            .Select(MapCartItem)
            .ToList();
    }

    public async Task AddToCartAsync(
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

        if (connection.State != System.Data.ConnectionState.Open)
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

    public async Task UpdateQuantityAsync(
        Guid userId,
        Guid cartItemId,
        int quantity,
        CancellationToken cancellationToken)
    {
        var cartItem = await context.CartItems
            .Include(x => x.Variant)
            .FirstOrDefaultAsync(
                x =>
                    x.Id == cartItemId &&
                    x.UserId == userId,
                cancellationToken);

        if (cartItem is null)
        {
            throw new KeyNotFoundException(
                "Cart item not found.");
        }

        if (quantity <= 0)
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

        if (quantity > cartItem.Variant.StockQuantity)
        {
            throw new InvalidOperationException(
                $"Only {cartItem.Variant.StockQuantity} in stock.");
        }

        cartItem.Quantity = quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task RemoveFromCartAsync(
        Guid userId,
        Guid cartItemId,
        CancellationToken cancellationToken)
    {
        var affectedRows = await context.CartItems
            .Where(x =>
                x.Id == cartItemId &&
                x.UserId == userId)
            .ExecuteDeleteAsync(
                cancellationToken);

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException(
                "Cart item not found.");
        }
    }

    public async Task ClearCartAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        await context.CartItems
            .Where(x => x.UserId == userId)
            .ExecuteDeleteAsync(
                cancellationToken);
    }

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
            .ExecuteDeleteAsync(
                cancellationToken);
    }

    private static CartItemDto MapCartItem(
        Domain.Entities.CartItem item)
    {
        var variant = item.Variant;

        var product = variant.Product;

        var category = product?.Category is null
            ? null
            : new CartCategoryDto(
                product.Category.Id,
                product.Category.Name
            );

        var images = product?.Images
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.SortOrder)
            .Select(x => new CartProductImageDto(
                x.Id,
                x.ImageUrl,
                x.IsPrimary,
                x.SortOrder
            ))
            .ToList()
            ?? [];

        var productDto = product is null
            ? null
            : new CartProductDto(
                product.Id,
                product.Name,
                product.BasePrice,
                category,
                images
            );

        var variantDto = new CartVariantDto(
            variant.Id,
            variant.ProductId,
            variant.Sku,
            variant.Color,
            variant.Size,
            variant.Price,
            variant.StockQuantity,
            variant.IsActive,
            productDto
        );

        return new CartItemDto(
            item.Id,
            item.Quantity,
            item.VariantId,
            variantDto,
            item.CreatedAt,
            item.UpdatedAt
        );
    }
}
