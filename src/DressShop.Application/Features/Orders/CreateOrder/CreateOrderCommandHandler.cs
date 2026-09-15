using System.Text.Json;
using DressShop.Application.Abstractions;
using DressShop.Application.Features.Orders.DTOs;
using DressShop.Application.Features.Orders.Services;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Orders.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IApplicationDbContext context,
    ITransactionManager transactionManager,
    IStockService stockService,
    ILoyaltyService loyaltyService,
    INotificationService notificationService
) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Request.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "An order must contain at least one item.");
        }

        if (request.Request.Items.Any(x => x.Quantity <= 0))
        {
            throw new InvalidOperationException(
                "Order item quantity must be greater than zero.");
        }

        var duplicateVariant = request.Request.Items
            .GroupBy(x => x.VariantId)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicateVariant is not null)
        {
            throw new InvalidOperationException(
                $"Variant '{duplicateVariant.Key}' appears more than once.");
        }

        await using var transaction =
            await transactionManager.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var variantIds = request.Request.Items
                .Select(x => x.VariantId)
                .ToList();

            var variants = await context.ProductVariants
                .Include(v => v.Product)
                .Where(v => variantIds.Contains(v.Id))
                .ToListAsync(cancellationToken);

            if (variants.Count != variantIds.Count)
            {
                throw new InvalidOperationException(
                    "One or more product variants could not be found.");
            }

            var variantById = variants.ToDictionary(v => v.Id);

            var pricingItems =
                new List<(ProductVariant Variant, int Quantity)>();

            foreach (var item in request.Request.Items)
            {
                var variant = variantById[item.VariantId];

                if (!variant.IsActive)
                {
                    throw new InvalidOperationException(
                        $"Product variant '{variant.Id}' is no longer available.");
                }

                pricingItems.Add(
                    (variant, item.Quantity));
            }

            var pendingDiscount =
                await loyaltyService.GetPendingDiscountAsync(
                    request.UserId,
                    cancellationToken);

            var totals = OrderPricingService.Calculate(
                pricingItems,
                pendingDiscount.Amount,
                pendingDiscount.FreeShipping);

            foreach (var item in request.Request.Items)
            {
                var success = await stockService.DecrementAsync(
                    item.VariantId,
                    item.Quantity,
                    cancellationToken);

                if (!success)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for variant '{item.VariantId}'.");
                }
            }

            var now = DateTime.UtcNow;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Status = "pending",
                Subtotal = totals.Subtotal,
                ShippingAmount = totals.Shipping,
                DiscountAmount = totals.Discount,
                Total = totals.Total,
                ShippingAddress = JsonSerializer.Serialize(
                    request.Request.ShippingAddress),
                CreatedAt = now,
                UpdatedAt = now
            };

            foreach (var item in request.Request.Items)
            {
                var variant = variantById[item.VariantId];

                var unitPrice =
                    OrderPricingService.ResolveUnitPrice(variant);

                order.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = variant.ProductId,
                    VariantId = variant.Id,
                    ProductName = variant.Product.Name,
                    VariantDescription =
                        BuildVariantDescription(variant),
                    UnitPrice = unitPrice,
                    Quantity = item.Quantity,
                    CreatedAt = now
                });
            }

            context.Orders.Add(order);

            await context.SaveChangesAsync(cancellationToken);

            await loyaltyService.ConsumePendingRedemptionsAsync(
                request.UserId,
                order.Id,
                pendingDiscount.TransactionIds,
                cancellationToken);

            await loyaltyService.EarnPointsAsync(
                request.UserId,
                order.Id,
                order.Total,
                cancellationToken);

            // Remove purchased variants from the user's cart.
            // This is intentionally done here instead of using ICartService.
            await context.CartItems
                .Where(x =>
                    x.UserId == request.UserId &&
                    variantIds.Contains(x.VariantId))
                .ExecuteDeleteAsync(
                    cancellationToken);

            await notificationService.CreateOrderConfirmedAsync(
                request.UserId,
                order.Id,
                order.Total,
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

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
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    private static string? BuildVariantDescription(
        ProductVariant variant)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(variant.Color))
        {
            parts.Add($"Color: {variant.Color}");
        }

        if (!string.IsNullOrWhiteSpace(variant.Size))
        {
            parts.Add($"Size: {variant.Size}");
        }

        return parts.Count == 0
            ? null
            : string.Join(", ", parts);
    }
}
