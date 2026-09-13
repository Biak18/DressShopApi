using DressShop.Application.Abstractions;
using DressShop.Application.Features.Loyalty.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Loyalty.GetTransactions;

public sealed record GetLoyaltyTransactionsQuery(
    Guid UserId,
    int Limit = 20
) : IRequest<IReadOnlyList<LoyaltyTransactionDto>>;

public sealed class GetLoyaltyTransactionsQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetLoyaltyTransactionsQuery, IReadOnlyList<LoyaltyTransactionDto>>
{
    public async Task<IReadOnlyList<LoyaltyTransactionDto>> Handle(
        GetLoyaltyTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 100);

        return await context.LoyaltyTransactions
            .AsNoTracking()
            .Where(t => t.UserId == request.UserId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .Select(t => new LoyaltyTransactionDto(
                t.Id,
                t.Points,
                t.Type,
                t.Description,
                t.OrderId,
                t.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
