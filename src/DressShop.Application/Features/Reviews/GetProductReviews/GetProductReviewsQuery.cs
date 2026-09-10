using DressShop.Application.Features.Reviews.DTOs;
using MediatR;

namespace DressShop.Application.Features.Reviews.GetProductReviews;

public record GetProductReviewsQuery(
    Guid ProductId
) : IRequest<IReadOnlyList<ReviewDto>>;
