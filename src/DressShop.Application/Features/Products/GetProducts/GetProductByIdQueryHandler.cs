using DressShop.Application.Abstractions;
using DressShop.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Products.GetProducts;

public class GetProductByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    public async Task<ProductDetailDto> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await context.Products
    .AsNoTracking()
    .Where(p => p.Id == request.Id)
    .Select(p => new ProductDetailDto(
        p.Id,
        p.Name,
        p.Slug,
        p.BasePrice,
        p.Description,
        p.Style,
        p.Occasion,
        p.CategoryId,
        p.Category != null ? p.Category.Name : null,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt,

        p.Images
            .OrderBy(i => i.SortOrder)
            .Select(i => new ProductImageDto(
                i.Id,
                i.ProductId,
                i.ImageUrl,
                i.AltText,
                i.SortOrder,
                i.IsPrimary
            ))
            .ToList(),

        p.Variants
            .Where(v => v.IsActive)
            .OrderBy(v => v.Color)
            .ThenBy(v => v.Size)
            .Select(v => new ProductVariantDto(
                v.Id,
                v.ProductId,
                v.Sku,
                v.Color,
                v.Size,
                v.Price,
                v.StockQuantity,
                v.IsActive
            ))
            .ToList()
    ))
    .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"Product with ID {request.Id} not found.");
        }

        return product;
    }
}
