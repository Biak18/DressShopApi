namespace DressShop.Application.Abstractions;

public interface IAssistantClient
{
    Task<AssistantReply> ChatAsync(
        IReadOnlyList<AssistantMessage> messages,
        CancellationToken cancellationToken);
}

public sealed record AssistantMessage(
    string Role,
    string Content
);

public sealed record AssistantReply(
    string Text
);
