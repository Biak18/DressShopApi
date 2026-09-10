using DressShop.Application.Abstractions;
using DressShop.Application.Features.Reviews.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Reviews.GetProductReviews;

public class GetProductReviewsQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetProductReviewsQuery, IReadOnlyList<ReviewDto>>
{
    public async Task<IReadOnlyList<ReviewDto>> Handle(
        GetProductReviewsQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == request.ProductId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto(
                r.Id,
                r.UserId,
                r.ProductId,
                r.OrderItemId,
                r.Rating,
                r.Body,
                r.CreatedAt,
                r.UpdatedAt,
                r.User.FullName,
                r.User.AvatarUrl
            ))
            .ToListAsync(cancellationToken);
    }
}
