namespace DressShop.Application.Features.Orders.DTOs;

public record OrderDto(
    Guid Id,
    Guid UserId,
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
