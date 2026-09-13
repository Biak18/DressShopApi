using DressShop.Application.Abstractions;
using DressShop.Application.Features.Categories.DTOs;
using DressShop.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Favorites.GetWishlist;

public sealed class GetWishlistQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetWishlistQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(
        GetWishlistQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Favorites
            .AsNoTracking()
            .Where(f => f.UserId == request.UserId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => f.Product)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Slug,
                p.BasePrice,
                p.CategoryId,

                p.Category == null
                    ? null
                    : new CategoryDto(
                        p.Category.Id,
                        p.Category.Name,
                        p.Category.Slug,
                        p.Category.Description,
                        p.Category.ImageUrl,
                        p.Category.SortOrder ?? 0,
                        p.Category.IsActive,
                        p.Category.CreatedAt,
                        p.Category.UpdatedAt
                    ),

                p.Description,
                p.Style,
                p.Occasion,
                p.IsActive,
                p.CreatedAt,
                p.UpdatedAt,

                p.Images
                    .OrderByDescending(i => i.IsPrimary)
                    .ThenBy(i => i.SortOrder)
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
            .ToListAsync(cancellationToken);
    }
}
