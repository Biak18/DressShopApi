using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Favorites.GetFavoriteStatus;

public class GetFavoriteStatusQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetFavoriteStatusQuery, bool>
{
    public async Task<bool> Handle(
        GetFavoriteStatusQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Favorites
            .AsNoTracking()
            .AnyAsync(
                f =>
                    f.UserId == request.UserId &&
                    f.ProductId == request.ProductId,
                cancellationToken);
    }
}
