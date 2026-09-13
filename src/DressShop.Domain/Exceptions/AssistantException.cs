namespace DressShop.Domain.Exceptions;

/// <summary>
/// Thrown when a downstream AI provider call fails (missing key,
/// provider rejection, empty response). The API layer maps this to
/// HTTP 502 with the message visible to the client, so mobile and
/// API consumers can diagnose configuration vs. provider issues.
/// Never include secrets or stack traces in the message.
/// </summary>
public sealed class AssistantException(string message) : Exception(message);
