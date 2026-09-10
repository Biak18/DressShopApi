using System.Text.Json;

namespace DressShop.Application.Abstractions;

public interface IProfilesClient
{
    Task<JsonElement?> GetCurrentAsync(
        string userId,
        string accessToken,
        CancellationToken cancellationToken = default);
}
