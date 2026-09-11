using DressShop.Application.Features.Favorites.DTOs;
using MediatR;

namespace DressShop.Application.Features.Favorites.GetProductFavorite;

public record GetProductFavoriteQuery(Guid UserId, Guid ProductId) : IRequest<FavoriteProduct>;
