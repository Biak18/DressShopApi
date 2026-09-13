namespace DressShop.Application.Abstractions;

public interface IAuthClient
{
    Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<AuthResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken);
}

public sealed record AuthResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
