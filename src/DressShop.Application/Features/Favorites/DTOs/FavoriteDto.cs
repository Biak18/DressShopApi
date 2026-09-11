namespace DressShop.Application.Features.Favorites.DTOs;

public record FavoriteDto(
    Guid UserId,
    Guid ProductId,
    DateTime CreatedAt
);
