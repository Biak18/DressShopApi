namespace DressShop.Application.Features.Loyalty.DTOs;

public sealed record LoyaltyAccountDto(
    int Points,
    string Tier,
    DateTime UpdatedAt
);

public sealed record LoyaltyTransactionDto(
    Guid Id,
    int Points,
    string Type,
    string? Description,
    Guid? OrderId,
    DateTime CreatedAt
);
