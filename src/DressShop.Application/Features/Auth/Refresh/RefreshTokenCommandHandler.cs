using DressShop.Application.Abstractions;
using MediatR;

namespace DressShop.Application.Features.Auth.Refresh;

public sealed class RefreshTokenCommandHandler(
    IAuthClient authClient
) : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    public Task<AuthResult> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        return authClient.RefreshAsync(
            request.RefreshToken,
            cancellationToken);
    }
}
