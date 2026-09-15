using DressShop.Application.Abstractions;
using DressShop.Application.Features.Loyalty.DTOs;
using DressShop.Application.Features.Loyalty.GetAccount;
using DressShop.Application.Features.Loyalty.GetTransactions;
using DressShop.Application.Features.Loyalty.Redeem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/loyalty")]
[EnableRateLimiting("api")]
[Authorize]
public sealed class LoyaltyController(
    ISender sender,
    ICurrentUser currentUser
) : ControllerBase
{
    [HttpGet("account")]
    public async Task<ActionResult<LoyaltyAccountDto>> GetAccount(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new GetLoyaltyAccountQuery(userId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<IReadOnlyList<LoyaltyTransactionDto>>> GetTransactions(
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new GetLoyaltyTransactionsQuery(userId, limit),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("redeem")]
    public async Task<ActionResult<LoyaltyAccountDto>> Redeem(
        RedeemRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new RedeemPointsCommand(
                userId,
                request.Points,
                request.Description),
            cancellationToken);

        return Ok(result);
    }
}

public sealed record RedeemRequest(
    int Points,
    string? Description);
