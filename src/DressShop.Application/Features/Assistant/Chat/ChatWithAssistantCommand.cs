using DressShop.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace DressShop.Application.Features.Assistant.Chat;

public sealed record ChatMessageDto(
    string Role,
    string Content
);

public sealed record ChatWithAssistantCommand(
    string Input,
    IReadOnlyList<ChatMessageDto>? History
) : IRequest<string>;

public sealed class ChatWithAssistantCommandValidator
    : AbstractValidator<ChatWithAssistantCommand>
{
    public ChatWithAssistantCommandValidator()
    {
        RuleFor(x => x.Input).NotEmpty().MaximumLength(2000);
    }
}

public sealed class ChatWithAssistantCommandHandler(
    IAssistantClient assistantClient
) : IRequestHandler<ChatWithAssistantCommand, string>
{
    public async Task<string> Handle(
        ChatWithAssistantCommand request,
        CancellationToken cancellationToken)
    {
        var messages = new List<AssistantMessage>();

        if (request.History is not null)
        {
            foreach (var h in request.History.TakeLast(8))
            {
                if (!string.IsNullOrWhiteSpace(h.Content))
                {
                    messages.Add(new AssistantMessage(h.Role, h.Content));
                }
            }
        }

        messages.Add(new AssistantMessage("user", request.Input.Trim()));

        var reply = await assistantClient.ChatAsync(messages, cancellationToken);

        return reply.Text;
    }
}
