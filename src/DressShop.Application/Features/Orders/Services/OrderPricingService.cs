using DressShop.Domain.Entities;

namespace DressShop.Application.Features.Orders.Services;

public static class OrderPricingService
{
    public const decimal FreeShippingThreshold = 120m;
    public const decimal FlatShipping = 8m;

    public static decimal ResolveUnitPrice(
        ProductVariant variant)
    {
        return variant.Price
            ?? variant.Product.BasePrice;
    }

    public static OrderTotals Calculate(
        IReadOnlyList<(ProductVariant Variant, int Quantity)> items,
        decimal discount = 0,
        bool freeShipping = false)
    {
        discount = Math.Max(0, discount);

        var subtotal = items.Sum(item =>
            ResolveUnitPrice(item.Variant) * item.Quantity);

        var shippingBase =
            subtotal == 0
                ? 0
                : subtotal >= FreeShippingThreshold
                    ? 0
                    : FlatShipping;

        var shipping = freeShipping
            ? 0
            : shippingBase;

        var total = Math.Max(
            0,
            subtotal +
            shipping -
            Math.Min(discount, subtotal + shipping)
        );

        var itemCount = items.Sum(
            item => item.Quantity);

        var isFreeShipping =
            (subtotal >= FreeShippingThreshold && subtotal > 0)
            || freeShipping;

        return new OrderTotals(
            subtotal,
            shipping,
            discount,
            total,
            itemCount,
            isFreeShipping
        );
    }
}

public record OrderTotals(
    decimal Subtotal,
    decimal Shipping,
    decimal Discount,
    decimal Total,
    int ItemCount,
    bool IsFreeShipping
);
