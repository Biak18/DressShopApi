using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Favorites.GetFavoriteIds;

public class GetFavoriteIdsQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetFavoriteIdsQuery, IReadOnlyList<Guid>>
{
    public async Task<IReadOnlyList<Guid>> Handle(
        GetFavoriteIdsQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Favorites
            .AsNoTracking()
            .Where(f => f.UserId == request.UserId)
            .Select(f => f.ProductId)
            .ToListAsync(cancellationToken);
    }
}
