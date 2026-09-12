using DressShop.Application.Abstractions;
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
public class ReviewsController(ISender sender, ICurrentUser currentUser) : ControllerBase
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
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new GetReviewEligibilityQuery(
                productId,
                userId),
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
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new CreateReviewCommand(
                userId,
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
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new UpdateReviewCommand(
                reviewId,
                userId,
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
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        await sender.Send(
            new DeleteReviewCommand(
                reviewId,
                userId),
            cancellationToken);

        return NoContent();
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
