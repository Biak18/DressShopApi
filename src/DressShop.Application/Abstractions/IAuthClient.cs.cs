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

    Task<RegisterResult> RegisterAsync(
        string email,
        string password,
        string? fullName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Revokes the session for the given access token.
    /// Idempotent - an already-invalid token is treated as success.
    /// </summary>
    Task LogoutAsync(
        string accessToken,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sends a password-recovery email. Always succeeds to avoid
    /// disclosing whether an account exists.
    /// </summary>
    Task RequestPasswordResetAsync(
        string email,
        CancellationToken cancellationToken);
}

public sealed record RegisterResult(
    string? AccessToken,
    string? RefreshToken,
    int ExpiresIn,
    bool EmailConfirmationRequired);

public sealed record AuthResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
