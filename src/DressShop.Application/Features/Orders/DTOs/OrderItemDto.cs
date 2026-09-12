namespace DressShop.Application.Features.Orders.DTOs;

public record OrderItemDto(
    Guid Id,
    Guid? ProductId,
    Guid? VariantId,
    string ProductName,
    string? VariantDescription,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal
);
