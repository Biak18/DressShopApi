using DressShop.Application.Abstractions;
using DressShop.Application.Features.Orders.DTOs;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Orders.CancelOrder;

public sealed class CancelOrderCommandHandler(
    IApplicationDbContext context,
    ITransactionManager transactionManager,
    IStockService stockService,
    ILoyaltyService loyaltyService,
    INotificationService notificationService
) : IRequestHandler<CancelOrderCommand, OrderDto?>
{
    public async Task<OrderDto?> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await transactionManager.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var order = await context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(
                    o =>
                        o.Id == request.OrderId &&
                        o.UserId == request.UserId,
                    cancellationToken);

            if (order is null)
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return null;
            }

            if (order.Status is not ("pending" or "confirmed"))
            {
                throw new InvalidOperationException(
                    $"Order cannot be cancelled while it is '{order.Status}'.");
            }

            foreach (var item in order.Items)
            {
                if (item.VariantId is not Guid variantId)
                {
                    continue;
                }

                await stockService.RestoreAsync(
                    variantId,
                    item.Quantity,
                    cancellationToken);
            }

            await loyaltyService.RestoreOrderLoyaltyAsync(
                request.UserId,
                order.Id,
                cancellationToken);

            order.Status = "cancelled";
            order.UpdatedAt = DateTime.UtcNow;

            await notificationService.CreateOrderCancelledAsync(
                request.UserId,
                order.Id,
                cancellationToken);

            await context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return MapOrder(order);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    private static OrderDto MapOrder(Order order)
    {
        return new OrderDto(
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
        );
    }
}
