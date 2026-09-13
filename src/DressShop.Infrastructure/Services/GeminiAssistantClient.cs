using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DressShop.Application.Abstractions;
using DressShop.Domain.Exceptions;
using Microsoft.Extensions.Configuration;

namespace DressShop.Infrastructure.Services;

/// <summary>
/// Stylist chat via Google Gemini. Key comes from configuration
/// (<c>Gemini:ApiKey</c>, optional <c>Gemini:Model</c>) so it never
/// leaves the server. Calls fail fast with a clear message when
/// unconfigured instead of returning a generic 500.
/// </summary>
public sealed class GeminiAssistantClient(
    HttpClient httpClient,
    IConfiguration configuration
) : IAssistantClient
{
    private const string DefaultModel = "gemini-2.0-flash";

    private const string SystemPrompt =
        "You are a warm, concise stylist for LUNE, a premium dress boutique. " +
        "Recommend dresses by occasion, style, and color. Keep replies under 120 words. " +
        "Never invent prices or products.";

    public async Task<AssistantReply> ChatAsync(
        IReadOnlyList<AssistantMessage> messages,
        CancellationToken cancellationToken)
    {
        var apiKey = configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new AssistantException(
                "AI assistant is not configured (Gemini:ApiKey).");
        }

        var model = configuration["Gemini:Model"] ?? DefaultModel;

        var contents = messages
            .Where(m => !string.IsNullOrWhiteSpace(m.Content))
            .Select(m => new GeminiContent(
                m.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase)
                    ? "model"
                    : "user",
                [new GeminiPart(m.Content.Trim())]))
            .ToList();

        if (contents.Count == 0)
        {
            throw new AssistantException("Message is required.");
        }

        var response = await httpClient.PostAsJsonAsync(
            $"v1beta/models/{model}:generateContent?key={apiKey}",
            new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = SystemPrompt } }
                },
                contents,
                generationConfig = new { maxOutputTokens = 512 }
            },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new AssistantException(
                $"AI provider rejected the request ({(int)response.StatusCode}).");
        }

        var payload = await response.Content.ReadFromJsonAsync<GeminiResponse>(
            cancellationToken);

        var text = string.Concat(
            (payload?.Candidates ?? [])
                .SelectMany(c => c.Content?.Parts ?? [])
                .Select(p => p.Text));

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new AssistantException(
                "AI returned an empty response (prompt may have been blocked).");
        }

        return new AssistantReply(text.Trim());
    }

    private sealed record GeminiContent(
        string Role,
        IReadOnlyList<GeminiPart> Parts
    );

    private sealed record GeminiPart(
        [property: JsonPropertyName("text")]
        string Text
    );

    private sealed record GeminiResponse(
        [property: JsonPropertyName("candidates")]
        IReadOnlyList<GeminiCandidate> Candidates
    );

    private sealed record GeminiCandidate(
        [property: JsonPropertyName("content")]
        GeminiContent? Content
    );
}
