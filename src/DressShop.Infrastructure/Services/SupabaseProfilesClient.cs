using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DressShop.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DressShop.Infrastructure.Services;

public sealed class SupabaseProfilesClient(HttpClient httpClient, IConfiguration configuration) : IProfilesClient
{
    private readonly string? anonKey = configuration["Supabase:AnonKey"];

    public async Task<JsonElement?> GetCurrentAsync(
        string userId,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var query = $"rest/v1/profiles?id=eq.{Uri.EscapeDataString(userId)}&select=*";
        using var request = new HttpRequestMessage(HttpMethod.Get, query);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        if (!string.IsNullOrWhiteSpace(anonKey))
        {
            request.Headers.Add("apikey", anonKey);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var profiles = await response.Content.ReadFromJsonAsync<JsonElement[]>(cancellationToken);
        return profiles is { Length: > 0 } ? profiles[0] : null;
    }
}
