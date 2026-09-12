namespace DressShop.Application.Features.Orders.DTOs;

public record CreateOrderItemRequest(
    Guid VariantId,
    int Quantity
);
