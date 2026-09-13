using DressShop.Application.Abstractions;
using DressShop.Application.Features.Cart.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Cart.GetCart;

public sealed class GetCartQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<
    GetCartQuery,
    IReadOnlyList<CartItemDto>>
{
    public async Task<IReadOnlyList<CartItemDto>> Handle(
        GetCartQuery request,
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
            .Where(x => x.UserId == request.UserId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return items
            .Select(MapCartItem)
            .ToList();
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
