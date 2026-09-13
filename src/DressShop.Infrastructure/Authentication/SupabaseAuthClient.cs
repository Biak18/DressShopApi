using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DressShop.Application.Abstractions;

namespace DressShop.Infrastructure.Authentication;

public sealed class SupabaseAuthClient(HttpClient httpClient) : IAuthClient
{
    public async Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "token?grant_type=password",
            new { email, password },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var payload =
            await response.Content.ReadFromJsonAsync<SupabaseTokenResponse>(
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Supabase returned an empty token response.");

        return new AuthResult(
            payload.AccessToken,
            payload.RefreshToken,
            payload.ExpiresIn);
    }

    public async Task<AuthResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "token?grant_type=refresh_token",
            new { refresh_token = refreshToken },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException(
                "Invalid or expired refresh token.");
        }

        var payload =
            await response.Content.ReadFromJsonAsync<SupabaseTokenResponse>(
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Supabase returned an empty token response.");

        return new AuthResult(
            payload.AccessToken,
            payload.RefreshToken,
            payload.ExpiresIn);
    }

    private sealed record SupabaseTokenResponse(
        [property: JsonPropertyName("access_token")]
        string AccessToken,

        [property: JsonPropertyName("refresh_token")]
        string RefreshToken,

        [property: JsonPropertyName("expires_in")]
        int ExpiresIn);
}
