using DressShop.Application.Abstractions;
using DressShop.Application.Features.Categories.DTOs;
using DressShop.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Products.GetProducts;

public class GetProductsQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetProductsQuery, PaginatedProductsDto>
{
    public async Task<PaginatedProductsDto> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Products
            .AsNoTracking()
            //.Include(p => p.Category)
            //.Include(p => p.Images)
            //.Include(p => p.Variants)
            .Where(p => p.IsActive == request.IsActive);

        // Category ID
        if (request.CategoryId.HasValue)
        {
            query = query.Where(
                p => p.CategoryId == request.CategoryId.Value);
        }

        // Category slug
        if (!string.IsNullOrWhiteSpace(request.CategorySlug))
        {
            query = query.Where(
                p => p.Category != null &&
                     p.Category.Slug == request.CategorySlug);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            var pattern = $"%{search}%";

            query = query.Where(p =>
                EF.Functions.Like(p.Name, pattern) ||
                EF.Functions.Like(p.Description!, pattern) ||
                EF.Functions.Like(p.Style!, pattern) ||
                EF.Functions.Like(p.Occasion!, pattern) ||
                EF.Functions.Like(p.Slug, pattern) ||
                (p.Category != null &&
                 EF.Functions.Like(p.Category.Name, pattern))
            );
        }

        // Style
        if (!string.IsNullOrWhiteSpace(request.Style))
        {
            query = query.Where(
                p => p.Style == request.Style);
        }

        // Occasion
        if (!string.IsNullOrWhiteSpace(request.Occasion))
        {
            query = query.Where(
                p => p.Occasion == request.Occasion);
        }

        // Minimum price
        if (request.MinPrice.HasValue)
        {
            query = query.Where(
                p => p.BasePrice >= request.MinPrice.Value);
        }

        // Maximum price
        if (request.MaxPrice.HasValue)
        {
            query = query.Where(
                p => p.BasePrice <= request.MaxPrice.Value);
        }

        // Color
        if (!string.IsNullOrWhiteSpace(request.Color))
        {
            query = query.Where(p =>
                p.Variants.Any(v =>
                    v.IsActive &&
                    v.Color == request.Color));
        }

        // Size
        if (!string.IsNullOrWhiteSpace(request.Size))
        {
            query = query.Where(p =>
                p.Variants.Any(v =>
                    v.IsActive &&
                    v.Size == request.Size));
        }

        // In stock
        if (request.InStock == true)
        {
            query = query.Where(p =>
                p.Variants.Any(v =>
                    v.IsActive &&
                    v.StockQuantity > 0));
        }

        // Sorting
        if (request.Sort == "top_rated")
        {
            query = query
                .OrderByDescending(p =>
                    p.Reviews
                        .Average(r => (double?)r.Rating) ?? 0)
                .ThenByDescending(p => p.CreatedAt);
        }
        else
        {
            query = request.Sort switch
            {
                "price_asc" =>
                    query.OrderBy(p => p.BasePrice),

                "price_desc" =>
                    query.OrderByDescending(p => p.BasePrice),

                "newest" or null =>
                    query.OrderByDescending(p => p.CreatedAt),

                _ =>
                    query.OrderByDescending(p => p.CreatedAt)
            };
        }

        // Count before pagination
        var count = await query.CountAsync(cancellationToken);

        // Pagination
        var products = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
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

        return new PaginatedProductsDto(
            products,
            count,
            request.Page,
            request.PageSize
        );
    }
}
