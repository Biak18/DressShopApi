using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Favorites.DeleteFavorite;

public class DeleteFavoriteCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<DeleteFavoriteCommand>
{
    public async Task Handle(
        DeleteFavoriteCommand request,
        CancellationToken cancellationToken)
    {
        var favorite = await context.Favorites
            .FirstOrDefaultAsync(
                f =>
                    f.UserId == request.UserId &&
                    f.ProductId == request.ProductId,
                cancellationToken);

        if (favorite is null)
        {
            throw new KeyNotFoundException(
                "Favorite not found.");
        }

        context.Favorites.Remove(favorite);

        await context.SaveChangesAsync(cancellationToken);
    }
}
