using DressShop.Application.Abstractions;
using DressShop.Domain.Entities;
using DressShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Infrastructure.Services;

public sealed class LoyaltyService(
    AppDbContext context
) : ILoyaltyService
{
    public async Task<PendingLoyaltyDiscount> GetPendingDiscountAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var transactions = await context.LoyaltyTransactions
            .AsNoTracking()
            .Where(t =>
                t.UserId == userId &&
                t.Type == "redeem" &&
                t.OrderId == null)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        decimal discount = 0;
        var freeShipping = false;

        foreach (var transaction in transactions)
        {
            switch (transaction.Points)
            {
                case -200:
                    discount += 10;
                    break;

                case -400:
                    discount += 25;
                    break;

                case -800:
                    freeShipping = true;
                    break;
            }
        }

        return new PendingLoyaltyDiscount(
            discount,
            freeShipping,
            transactions
                .Select(t => t.Id)
                .ToList()
        );
    }

    public async Task ConsumePendingRedemptionsAsync(
        Guid userId,
        Guid orderId,
        IReadOnlyList<Guid> transactionIds,
        CancellationToken cancellationToken)
    {
        if (transactionIds.Count == 0)
        {
            return;
        }

        var affectedRows = await context.LoyaltyTransactions
            .Where(t =>
                t.UserId == userId &&
                t.Type == "redeem" &&
                t.OrderId == null &&
                transactionIds.Contains(t.Id))
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        t => t.OrderId,
                        orderId),
                cancellationToken);

        if (affectedRows != transactionIds.Count)
        {
            throw new InvalidOperationException(
                "One or more loyalty redemptions have already been consumed.");
        }
    }

    public async Task EarnPointsAsync(
        Guid userId,
        Guid orderId,
        decimal orderTotal,
        CancellationToken cancellationToken)
    {
        var points = (int)Math.Floor(orderTotal);

        if (points <= 0)
        {
            return;
        }

        var account = await context.LoyaltyAccounts
            .FirstOrDefaultAsync(
                a => a.UserId == userId,
                cancellationToken);

        if (account is null)
        {
            account = new LoyaltyAccount
            {
                UserId = userId,
                Points = 0,
                Tier = "bronze",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _ = context.LoyaltyAccounts.Add(account);
        }

        account.Points += points;
        account.Tier = GetTier(account.Points);
        account.UpdatedAt = DateTime.UtcNow;

        _ = context.LoyaltyTransactions.Add(
            new LoyaltyTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Points = points,
                Type = "earn",
                Description = $"Points earned from order #{orderId.ToString()[..8].ToUpperInvariant()}",
                OrderId = orderId,
                CreatedAt = DateTime.UtcNow
            });
    }

    private static string GetTier(int points)
    {
        return points switch
        {
            >= 2000 => "platinum",
            >= 1000 => "gold",
            >= 400 => "silver",
            _ => "bronze"
        };
    }
}
