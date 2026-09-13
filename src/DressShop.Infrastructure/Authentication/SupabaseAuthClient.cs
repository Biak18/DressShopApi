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

    public async Task<RegisterResult> RegisterAsync(
        string email,
        string password,
        string? fullName,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "signup",
            new
            {
                email,
                password,
                data = new { full_name = fullName?.Trim() }
            },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                "Registration failed. The email may already be in use.");
        }

        var payload =
            await response.Content.ReadFromJsonAsync<SupabaseSignupResponse>(
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Supabase returned an empty signup response.");

        // Email confirmation ON -> no session: user must confirm first.
        if (string.IsNullOrWhiteSpace(payload.AccessToken))
        {
            return new RegisterResult(null, null, 0, true);
        }

        return new RegisterResult(
            payload.AccessToken,
            payload.RefreshToken!,
            payload.ExpiresIn,
            false);
    }

    public async Task LogoutAsync(
        string accessToken,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "logout");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer",
            accessToken);

        var response = await httpClient.SendAsync(request, cancellationToken);

        // Already-invalid tokens are treated as logged out.
        if (!response.IsSuccessStatusCode &&
            response.StatusCode != System.Net.HttpStatusCode.Unauthorized &&
            response.StatusCode != System.Net.HttpStatusCode.Forbidden &&
            response.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException("Sign out failed.");
        }
    }

    public async Task RequestPasswordResetAsync(
        string email,
        CancellationToken cancellationToken)
    {
        // GoTrue returns 200 even for unknown emails - no enumeration leak.
        _ = await httpClient.PostAsJsonAsync(
            "recover",
            new { email },
            cancellationToken);
    }
    private sealed record SupabaseSignupResponse(
        [property: JsonPropertyName("access_token")]
        string? AccessToken,

        [property: JsonPropertyName("refresh_token")]
        string? RefreshToken,

        [property: JsonPropertyName("expires_in")]
        int ExpiresIn);

    private sealed record SupabaseTokenResponse(
        [property: JsonPropertyName("access_token")]
        string AccessToken,

        [property: JsonPropertyName("refresh_token")]
        string RefreshToken,

        [property: JsonPropertyName("expires_in")]
        int ExpiresIn);
}
