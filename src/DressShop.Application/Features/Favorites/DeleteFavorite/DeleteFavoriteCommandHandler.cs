using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Favorites.DeleteFavorite;

public class DeleteFavoriteCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteFavoriteCommand>
{
    public async Task Handle(DeleteFavoriteCommand request, CancellationToken cancellationToken)
    {
        var favorite = await context.Favorites
            .FirstOrDefaultAsync(f => f.Id == request.Id,
            cancellationToken);

        if (favorite == null)
        {
            throw new KeyNotFoundException(
               $"Favorite with ID '{request.Id}' was not found.");
        }

        context.Favorites.Remove(favorite);
        await context.SaveChangesAsync();
    }
}
