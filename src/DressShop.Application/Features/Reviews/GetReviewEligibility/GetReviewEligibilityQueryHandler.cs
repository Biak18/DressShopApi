using DressShop.Application.Abstractions;
using DressShop.Application.Features.Reviews.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Reviews.GetReviewEligibility;

public class GetReviewEligibilityQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetReviewEligibilityQuery, ReviewEligibilityDto>
{
    public async Task<ReviewEligibilityDto> Handle(
        GetReviewEligibilityQuery request,
        CancellationToken cancellationToken)
    {
        var orderItemId = await context.OrderItems
            .AsNoTracking()
            .Where(oi =>
                oi.ProductId == request.ProductId &&
                oi.Order.UserId == request.UserId &&
                oi.Order.Status != "cancelled")
            .Select(oi => (Guid?)oi.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return orderItemId.HasValue
            ? new ReviewEligibilityDto(true, orderItemId.Value)
            : new ReviewEligibilityDto(false, null);
    }
}
