using DressShop.Application.Abstractions;
using DressShop.Application.Features.Favorites.DTOs;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Favorites.CreateFavorite;

public class CreateFavoriteCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateFavoriteCommand, FavoriteDto>
{
    public async Task<FavoriteDto> Handle(CreateFavoriteCommand request, CancellationToken cancellationToken)
    {

        // Verify product exists
        var productExists = await context.Products
            .AsNoTracking()
            .AnyAsync(
                p => p.Id == request.ProductId,
                cancellationToken);

        if (!productExists)
        {
            throw new KeyNotFoundException(
                $"Product with ID {request.ProductId} not found.");
        }

        var favorite = new Favorite
        {
            ProductId = request.ProductId,
            UserId = request.UserId
        };

        context.Favorites.Add(favorite);
        await context.SaveChangesAsync();

        return new FavoriteDto(
            favorite.Id,
            favorite.CreatedAt,
            favorite.UserId,
            favorite.ProductId
            );

    }
}
