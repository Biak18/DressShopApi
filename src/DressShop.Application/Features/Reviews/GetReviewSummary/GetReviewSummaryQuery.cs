using DressShop.Application.Features.Reviews.DTOs;
using MediatR;

namespace DressShop.Application.Features.Reviews.GetReviewSummary;

public record GetReviewSummaryQuery(
    Guid ProductId
) : IRequest<ReviewSummaryDto>;
