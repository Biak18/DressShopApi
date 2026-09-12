using DressShop.Application.Abstractions;
using DressShop.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.GetLowStock;

public sealed class GetAdminLowStockQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<
    GetAdminLowStockQuery,
    IReadOnlyList<AdminLowStockItemDto>>
{
    public async Task<IReadOnlyList<AdminLowStockItemDto>> Handle(
        GetAdminLowStockQuery request,
        CancellationToken cancellationToken)
    {
        var threshold = Math.Max(request.Threshold, 0);
        var limit = Math.Clamp(request.Limit, 1, 100);

        var variants = await context.ProductVariants
            .AsNoTracking()
            .Include(variant => variant.Product)
            .Where(variant =>
                variant.StockQuantity <= threshold)
            .OrderBy(variant => variant.StockQuantity)
            .ThenBy(variant => variant.Product.Name)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return variants
            .Select(variant => new AdminLowStockItemDto(
                variant.Id,
                variant.Sku,
                variant.Color,
                variant.Size,
                variant.StockQuantity,
                variant.Product is null
                    ? null
                    : new AdminLowStockProductDto(
                        variant.Product.Id,
                        variant.Product.Name,
                        variant.Product.Slug
                    )
            ))
            .ToList();
    }
}
