using DressShop.Application.Features.Reviews.DTOs;
using MediatR;

namespace DressShop.Application.Features.Reviews.GetReviewEligibility;

public record GetReviewEligibilityQuery(
    Guid ProductId,
    Guid UserId
) : IRequest<ReviewEligibilityDto>;
