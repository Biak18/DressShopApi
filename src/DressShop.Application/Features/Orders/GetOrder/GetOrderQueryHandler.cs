using DressShop.Application.Abstractions;
using DressShop.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Orders.GetOrder;

public sealed class GetOrderQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetOrderQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Where(o =>
                o.Id == request.OrderId &&
                o.UserId == request.UserId)
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
            .FirstOrDefaultAsync(cancellationToken);

        return order;
    }
}
