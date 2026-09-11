using DressShop.Application.Features.Favorites.DTOs;
using MediatR;

public record CreateFavoriteCommand(
    Guid UserId,
    Guid ProductId
) : IRequest<FavoriteDto>;
