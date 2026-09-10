using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DressShop.Application.Abstractions;

namespace DressShop.Infrastructure.Authentication;

public sealed class SupabaseAuthClient(HttpClient httpClient) : IAuthClient
{
    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "token?grant_type=password",
            new { email, password },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            // Supabase returns 400 with its own error body for bad credentials -
            // normalize that into a plain 401 rather than leaking Supabase's
            // response shape/status code through your own API.
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var payload = await response.Content.ReadFromJsonAsync<SupabaseTokenResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Supabase returned an empty token response.");

        return new AuthResult(payload.AccessToken, payload.RefreshToken, payload.ExpiresIn);
    }

    private sealed record SupabaseTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);
}
