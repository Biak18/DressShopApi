namespace DressShop.Application.Abstractions;

public interface IAuthClient
{
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
}

public sealed record AuthResult(string AccessToken, string RefreshToken, int ExpiresIn);
