using DressShop.Application.Abstractions;
using DressShop.Application.Features.Loyalty.DTOs;
using DressShop.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Loyalty.Redeem;

public sealed record RedeemPointsCommand(
    Guid UserId,
    int Points,
    string? Description
) : IRequest<LoyaltyAccountDto>;

public sealed class RedeemPointsCommandValidator
    : AbstractValidator<RedeemPointsCommand>
{
    // Must stay in sync with the discount tiers consumed at checkout:
    // 200 -> $10 off, 400 -> $25 off, 800 -> free shipping.
    private static readonly int[] AllowedTiers = [200, 400, 800];

    public RedeemPointsCommandValidator()
    {
        RuleFor(x => x.Points)
            .Must(p => AllowedTiers.Contains(p))
            .WithMessage("Only 200, 400, or 800 point rewards can be redeemed.");
    }
}

public sealed class RedeemPointsCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<RedeemPointsCommand, LoyaltyAccountDto>
{
    public async Task<LoyaltyAccountDto> Handle(
        RedeemPointsCommand request,
        CancellationToken cancellationToken)
    {
        var account = await context.LoyaltyAccounts
            .FirstOrDefaultAsync(
                a => a.UserId == request.UserId,
                cancellationToken);

        if (account is null || account.Points < request.Points)
        {
            throw new InvalidOperationException(
                "Not enough loyalty points for this reward.");
        }

        account.Points -= request.Points;
        account.Tier = GetTier(account.Points);
        account.UpdatedAt = DateTime.UtcNow;

        context.LoyaltyTransactions.Add(new LoyaltyTransaction
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Points = -request.Points,
            Type = "redeem",
            Description = request.Description?.Trim(),
            OrderId = null,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync(cancellationToken);

        return new LoyaltyAccountDto(
            account.Points,
            account.Tier,
            account.UpdatedAt);
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
