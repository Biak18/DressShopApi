using MediatR;

namespace DressShop.Application.Features.Reviews.DeleteReview;

public record DeleteReviewCommand(
    Guid ReviewId,
    Guid UserId
) : IRequest;
