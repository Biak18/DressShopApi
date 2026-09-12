using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.UpdateVariantStock;

public sealed class UpdateVariantStockCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<UpdateVariantStockCommand>
{
    public async Task Handle(
        UpdateVariantStockCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Quantity < 0)
        {
            throw new InvalidOperationException(
                "Stock cannot be negative.");
        }

        var variant = await context.ProductVariants
            .FirstOrDefaultAsync(
                variant => variant.Id == request.VariantId,
                cancellationToken);

        if (variant is null)
        {
            throw new KeyNotFoundException(
                "Product variant not found.");
        }

        variant.StockQuantity = request.Quantity;
        variant.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
}
