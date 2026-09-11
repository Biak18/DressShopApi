namespace DressShop.Application.Features.Favorites.DTOs;

public record FavoriteDto(
    Guid Id,
    DateTime CreatedAt,
    Guid UserId,
    Guid ProductId
    );
