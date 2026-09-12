using MediatR;

namespace DressShop.Application.Features.Favorites.GetFavoriteStatus;

public record GetFavoriteStatusQuery(
    Guid UserId,
    Guid ProductId
) : IRequest<bool>;
