using DressShop.Application.Abstractions;
using DressShop.Application.Features.Reviews.DTOs;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Reviews.CreateReview;

public class CreateReviewCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    public async Task<ReviewDto> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        // Validate rating
        if (request.Rating < 1 || request.Rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5.");
        }

        // Verify product exists
        var productExists = await context.Products
            .AsNoTracking()
            .AnyAsync(
                p => p.Id == request.ProductId,
                cancellationToken);

        if (!productExists)
        {
            throw new KeyNotFoundException(
                $"Product with ID {request.ProductId} not found.");
        }

        // Prevent duplicate review
        var alreadyReviewed = await context.Reviews
            .AsNoTracking()
            .AnyAsync(
                r =>
                    r.ProductId == request.ProductId &&
                    r.UserId == request.UserId,
                cancellationToken);

        if (alreadyReviewed)
        {
            throw new InvalidOperationException(
                "You have already reviewed this product.");
        }

        // Find a valid purchase
        var orderItemId = await context.OrderItems
            .AsNoTracking()
            .Where(oi =>
                oi.ProductId == request.ProductId &&
                oi.Order.UserId == request.UserId &&
                oi.Order.Status != "cancelled")
            .Select(oi => (Guid?)oi.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!orderItemId.HasValue)
        {
            throw new InvalidOperationException(
                "You must purchase this product before reviewing it.");
        }

        var now = DateTime.UtcNow;

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ProductId = request.ProductId,
            OrderItemId = orderItemId.Value,
            Rating = request.Rating,
            Body = request.Body?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Reviews.Add(review);

        await context.SaveChangesAsync(cancellationToken);

        // Return the same shape as GetProductReviews
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
