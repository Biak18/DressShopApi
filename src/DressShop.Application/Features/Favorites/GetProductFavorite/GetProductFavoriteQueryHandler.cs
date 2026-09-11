using DressShop.Application.Abstractions;
using DressShop.Application.Features.Favorites.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Favorites.GetProductFavorite;

public class GetProductFavoriteQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProductFavoriteQuery, FavoriteProduct>
{
    public async Task<FavoriteProduct> Handle(GetProductFavoriteQuery request, CancellationToken cancellationToken)
    {
        var productFav = await context.Favorites
            .AsNoTracking()
            .Where(f => f.UserId == request.UserId && f.ProductId == request.ProductId)
            .Select(f => new FavoriteProduct(
                f.ProductId)).FirstOrDefaultAsync();

        if (productFav is null)
        {
            throw new KeyNotFoundException("Not Found");
        }

        return productFav;
    }
}
