using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Reviews.DeleteReview;

public class DeleteReviewCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<DeleteReviewCommand>
{
    public async Task Handle(
        DeleteReviewCommand request,
        CancellationToken cancellationToken)
    {
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

        context.Reviews.Remove(review);

        await context.SaveChangesAsync(cancellationToken);
    }
}
