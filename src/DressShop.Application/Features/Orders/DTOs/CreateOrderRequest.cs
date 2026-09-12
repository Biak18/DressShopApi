namespace DressShop.Application.Features.Orders.DTOs;

public record CreateOrderRequest(
    IReadOnlyList<CreateOrderItemRequest> Items,
    string? ShippingAddress
);
