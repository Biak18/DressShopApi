using MediatR;

namespace DressShop.Application.Features.Favorites.GetFavoriteIds;

public record GetFavoriteIdsQuery(
    Guid UserId
) : IRequest<IReadOnlyList<Guid>>;
