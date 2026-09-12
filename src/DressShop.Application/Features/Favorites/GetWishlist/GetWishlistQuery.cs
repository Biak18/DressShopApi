using DressShop.Application.Features.Products.DTOs
using MediatR;

namespace DressShop.Application.Features.Favorites.GetWishlist;

public record GetWishlistQuery(
    Guid UserId
) : IRequest<IReadOnlyList<ProductDto>>;
