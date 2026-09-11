using MediatR;

namespace DressShop.Application.Features.Favorites.DeleteFavorite;

public record DeleteFavoriteCommand(
    Guid Id
    ) : IRequest;

