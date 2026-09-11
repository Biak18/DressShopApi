using DressShop.Application.Features.Favorites.DTOs;
using MediatR;

namespace DressShop.Application.Features.Favorites.CreateFavorite;

public record CreateFavoriteCommand(
    Guid UserId,
    Guid ProductId
) : IRequest<FavoriteDto>;
