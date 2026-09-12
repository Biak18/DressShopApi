namespace DressShop.Application.Features.Cart.DTOs;

public record AddCartItemRequest(
    Guid VariantId,
    int Quantity = 1
);
