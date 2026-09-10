using DressShop.Application.Abstractions;
using DressShop.Application.Features.Reviews.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Reviews.GetReviewSummary;

public class GetReviewSummaryQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetReviewSummaryQuery, ReviewSummaryDto>
{
    public async Task<ReviewSummaryDto> Handle(
        GetReviewSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var result = await context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == request.ProductId)
            .GroupBy(_ => 1)
            .Select(g => new ReviewSummaryDto(
                g.Average(r => (double)r.Rating),
                g.Count()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? new ReviewSummaryDto(0, 0);
    }
}
