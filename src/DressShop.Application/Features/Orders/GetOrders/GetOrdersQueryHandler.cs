using DressShop.Application.Abstractions;
using DressShop.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Orders.GetOrders;

public class GetOrdersQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == request.UserId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto(
                o.Id,
                o.UserId,
                o.Status,
                o.Subtotal,
                o.ShippingAmount,
                o.DiscountAmount,
                o.Total,
                o.ShippingAddress,
                o.CreatedAt,
                o.UpdatedAt,

                o.Items
                    .OrderBy(i => i.CreatedAt)
                    .Select(i => new OrderItemDto(
                        i.Id,
                        i.ProductId,
                        i.VariantId,
                        i.ProductName,
                        i.VariantDescription,
                        i.UnitPrice,
                        i.Quantity,
                        i.UnitPrice * i.Quantity
                    ))
                    .ToList()
            ))
            .ToListAsync(cancellationToken);

        return orders;
    }
}
