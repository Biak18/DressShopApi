using DressShop.Application.Abstractions;
using DressShop.Application.Features.Loyalty.DTOs;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Loyalty.GetAccount;

public sealed record GetLoyaltyAccountQuery(
    Guid UserId
) : IRequest<LoyaltyAccountDto>;

public sealed class GetLoyaltyAccountQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetLoyaltyAccountQuery, LoyaltyAccountDto>
{
    public async Task<LoyaltyAccountDto> Handle(
        GetLoyaltyAccountQuery request,
        CancellationToken cancellationToken)
    {
        var account = await context.LoyaltyAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.UserId == request.UserId,
                cancellationToken);

        // Lazily provision so the client never deals with 404s.
        if (account is null)
        {
            account = new LoyaltyAccount
            {
                UserId = request.UserId,
                Points = 0,
                Tier = "bronze",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.LoyaltyAccounts.Add(account);

            await context.SaveChangesAsync(cancellationToken);
        }

        return new LoyaltyAccountDto(
            account.Points,
            account.Tier,
            account.UpdatedAt);
    }
}
