namespace DressShop.Application.Abstractions;

public interface ICurrentUser
{
    Guid? UserId { get; }

    /// <summary>
    /// Email claim from the access token, if present.
    /// </summary>
    string? Email { get; }

    bool IsAuthenticated { get; }
}
