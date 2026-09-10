using System.Security.Claims;
using DressShop.Application.Features.Reviews.CreateReview;
using DressShop.Application.Features.Reviews.DeleteReview;
using DressShop.Application.Features.Reviews.GetProductReviews;
using DressShop.Application.Features.Reviews.GetReviewEligibility;
using DressShop.Application.Features.Reviews.GetReviewSummary;
using DressShop.Application.Features.Reviews.UpdateReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(ISender sender) : ControllerBase
{
    // GET: api/products/{productId}/reviews
    [HttpGet("products/{productId:guid}/reviews")]
    public async Task<IActionResult> GetReviews(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProductReviewsQuery(productId),
            cancellationToken);

        return Ok(result);
    }

    // GET: api/products/{productId}/reviews/summary
    [HttpGet("products/{productId:guid}/reviews/summary")]
    public async Task<IActionResult> GetSummary(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetReviewSummaryQuery(productId),
            cancellationToken);

        return Ok(result);
    }

    // GET: api/products/{productId}/reviews/eligibility
    [Authorize]
    [HttpGet("products/{productId:guid}/reviews/eligibility")]
    public async Task<IActionResult> GetEligibility(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new GetReviewEligibilityQuery(
                productId,
                userId.Value),
            cancellationToken);

        return Ok(result);
    }

    // POST: api/products/{productId}/reviews
    [Authorize]
    [HttpPost("products/{productId:guid}/reviews")]
    public async Task<IActionResult> CreateReview(
        Guid productId,
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new CreateReviewCommand(
                userId.Value,
                productId,
                request.Rating,
                request.Body),
            cancellationToken);

        return Created(
            $"/api/reviews/{result.Id}",
            result);
    }

    // PUT: api/reviews/{reviewId}
    [Authorize]
    [HttpPut("reviews/{reviewId:guid}")]
    public async Task<IActionResult> UpdateReview(
        Guid reviewId,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new UpdateReviewCommand(
                reviewId,
                userId.Value,
                request.Rating,
                request.Body),
            cancellationToken);

        return Ok(result);
    }

    // DELETE: api/reviews/{reviewId}
    [Authorize]
    [HttpDelete("reviews/{reviewId:guid}")]
    public async Task<IActionResult> DeleteReview(
        Guid reviewId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        await sender.Send(
            new DeleteReviewCommand(
                reviewId,
                userId.Value),
            cancellationToken);

        return NoContent();
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue("sub");

        return Guid.TryParse(userId, out var id)
            ? id
            : null;
    }
}

public record CreateReviewRequest(
    int Rating,
    string? Body
);

public record UpdateReviewRequest(
    int Rating,
    string? Body
);
