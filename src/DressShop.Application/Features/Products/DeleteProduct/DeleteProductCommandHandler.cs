using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Products.DeleteProduct;

public class DeleteProductCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.Id,
                cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"Product with ID '{request.Id}' was not found.");
        }

        context.Products.Remove(product);

        await context.SaveChangesAsync(cancellationToken);
    }
}
