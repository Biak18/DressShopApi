using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.UpdateProductActive;

public sealed class UpdateProductActiveCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<UpdateProductActiveCommand>
{
    public async Task Handle(
        UpdateProductActiveCommand request,
        CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(
                product => product.Id == request.ProductId,
                cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        product.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);
    }
}
