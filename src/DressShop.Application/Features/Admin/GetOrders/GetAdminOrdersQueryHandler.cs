using DressShop.Application.Abstractions;
using DressShop.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.GetOrders;

public sealed class GetAdminOrdersQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<
    GetAdminOrdersQuery,
    IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetAdminOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 100);

        var orders = await context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return orders
            .Select(order => new OrderDto(
                order.Id,
                order.UserId,
                order.Status,
                order.Subtotal,
                order.ShippingAmount,
                order.DiscountAmount,
                order.Total,
                order.ShippingAddress,
                order.CreatedAt,
                order.UpdatedAt,
                order.Items
                    .Select(item => new OrderItemDto(
                        item.Id,
                        item.ProductId,
                        item.VariantId,
                        item.ProductName,
                        item.VariantDescription,
                        item.UnitPrice,
                        item.Quantity,
                        item.UnitPrice * item.Quantity
                    ))
                    .ToList()
            ))
            .ToList();
    }
}
