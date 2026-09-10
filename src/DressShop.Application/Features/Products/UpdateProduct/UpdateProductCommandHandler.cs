using DressShop.Application.Abstractions;
using DressShop.Application.Features.Products.CreateProduct;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Products.UpdateProduct;

public class UpdateProductCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        UpdateProductCommand request,
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

        var slugExists = await context.Products
            .AnyAsync(
                p =>
                    p.Slug == request.Slug &&
                    p.Id != request.Id,
                cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException(
                "Product slug already exists.");
        }

        product.Name = request.Name;
        product.Slug = request.Slug;
        product.BasePrice = request.BasePrice;
        product.CategoryId = request.CategoryId;
        product.Description = request.Description;
        product.Occasion = request.Occasion;
        product.Style = request.Style;
        product.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Slug,
            product.BasePrice,
            product.CategoryId,
            product.Description,
            product.Occasion,
            product.Style,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt
        );
    }
}
