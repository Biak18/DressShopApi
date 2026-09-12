namespace DressShop.Application.Features.Orders.DTOs;

public record OrderDto(
    Guid Id,
    string Status,
    decimal Subtotal,
    decimal ShippingAmount,
    decimal DiscountAmount,
    decimal Total,
    string? ShippingAddress,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<OrderItemDto> Items
);
