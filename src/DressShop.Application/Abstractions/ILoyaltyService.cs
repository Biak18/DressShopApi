namespace DressShop.Application.Abstractions;

public interface ILoyaltyService
{
    Task<PendingLoyaltyDiscount> GetPendingDiscountAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task ConsumePendingRedemptionsAsync(
        Guid userId,
        Guid orderId,
        IReadOnlyList<Guid> transactionIds,
        CancellationToken cancellationToken);

    Task EarnPointsAsync(
        Guid userId,
        Guid orderId,
        decimal orderTotal,
        CancellationToken cancellationToken);

    Task RestoreOrderLoyaltyAsync(
    Guid userId,
    Guid orderId,
    CancellationToken cancellationToken);
}

public sealed record PendingLoyaltyDiscount(
    decimal Amount,
    bool FreeShipping,
    IReadOnlyList<Guid> TransactionIds
);
