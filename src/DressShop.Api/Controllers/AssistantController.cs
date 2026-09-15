using DressShop.Application.Abstractions;
using DressShop.Application.Features.Assistant.Chat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/assistant")]
[EnableRateLimiting("api")]
[Authorize]
public sealed class AssistantController(
    ISender sender,
    ICurrentUser currentUser
) : ControllerBase
{
    [HttpPost("chat")]
    public async Task<ActionResult<AssistantChatResponse>> Chat(
        AssistantChatRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var text = await sender.Send(
            new ChatWithAssistantCommand(
                request.Input,
                request.Messages),
            cancellationToken);

        return Ok(new AssistantChatResponse(text));
    }
}

public sealed record AssistantChatRequest(
    string Input,
    IReadOnlyList<ChatMessageDto>? Messages);

public sealed record AssistantChatResponse(
    string Text);
