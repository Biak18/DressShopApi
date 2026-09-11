using DressShop.Application.Abstractions;
using DressShop.Application.Features.Favorites.CreateFavorite;
using DressShop.Application.Features.Favorites.DTOs;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateFavoriteCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<CreateFavoriteCommand, FavoriteDto>
{
    public async Task<FavoriteDto> Handle(
        CreateFavoriteCommand request,
        CancellationToken cancellationToken)
    {
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
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ProductId = request.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        context.Favorites.Add(favorite);

        await context.SaveChangesAsync(cancellationToken);

        return new FavoriteDto(
            favorite.Id,
            favorite.CreatedAt,
            favorite.UserId,
            favorite.ProductId);
    }
}
