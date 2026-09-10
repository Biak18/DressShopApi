using DressShop.Application.Features.Reviews.DTOs;
using MediatR;

namespace DressShop.Application.Features.Reviews.UpdateReview;

public record UpdateReviewCommand(
    Guid ReviewId,
    Guid UserId,
    int Rating,
    string? Body
) : IRequest<ReviewDto>;
