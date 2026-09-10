using DressShop.Application.Abstractions;
using DressShop.Application.Features.Reviews.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Reviews.UpdateReview;

public class UpdateReviewCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    public async Task<ReviewDto> Handle(
        UpdateReviewCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Rating < 1 || request.Rating > 5)
        {
            throw new ArgumentException(
                "Rating must be between 1 and 5.");
        }

        var review = await context.Reviews
            .FirstOrDefaultAsync(
                r =>
                    r.Id == request.ReviewId &&
                    r.UserId == request.UserId,
                cancellationToken);

        if (review is null)
        {
            throw new KeyNotFoundException(
                "Review not found.");
        }

        review.Rating = request.Rating;
        review.Body = request.Body?.Trim();
        review.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return await context.Reviews
            .AsNoTracking()
            .Where(r => r.Id == review.Id)
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
            .SingleAsync(cancellationToken);
    }
}
