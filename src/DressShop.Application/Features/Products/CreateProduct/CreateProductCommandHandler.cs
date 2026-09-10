using DressShop.Application.Abstractions;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Products.CreateProduct;

public class CreateProductCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var slugExists = await context.Products
            .AnyAsync(
                p => p.Slug == request.Slug,
                cancellationToken);

        if (slugExists)
        {
            throw new Exception("Product slug already exists.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            BasePrice = request.BasePrice,
            CategoryId = request.CategoryId,
            Description = request.Description,
            Occasion = request.Occasion,
            Style = request.Style,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Products.Add(product);

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
