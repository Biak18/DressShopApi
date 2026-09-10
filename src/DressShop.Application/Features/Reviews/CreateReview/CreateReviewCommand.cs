using DressShop.Application.Features.Reviews.DTOs;
using MediatR;

namespace DressShop.Application.Features.Reviews.CreateReview;

public record CreateReviewCommand(
    Guid UserId,
    Guid ProductId,
    int Rating,
    string? Body
) : IRequest<ReviewDto>;
