using DressShop.Application.Abstractions;
using DressShop.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.GetStats;

public sealed class GetAdminStatsQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetAdminStatsQuery, AdminStatsDto>
{
    public async Task<AdminStatsDto> Handle(
        GetAdminStatsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await context.Products
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var orders = await context.Orders
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var lowStock = await context.ProductVariants
            .AsNoTracking()
            .CountAsync(
                variant => variant.StockQuantity <= 3,
                cancellationToken);

        var categories = await context.Categories
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var customers = await context.Profiles
            .AsNoTracking()
            .CountAsync(cancellationToken);

        return new AdminStatsDto(
            Products: products,
            Orders: orders,
            LowStock: lowStock,
            Categories: categories,
            Customers: customers
        );
    }
}
