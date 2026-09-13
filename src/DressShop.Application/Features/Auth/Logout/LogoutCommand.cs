using DressShop.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace DressShop.Application.Features.Auth.Logout;

public sealed record LogoutCommand(
    string AccessToken
) : IRequest;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
    }
}

public sealed class LogoutCommandHandler(IAuthClient authClient)
    : IRequestHandler<LogoutCommand>
{
    public Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
        => authClient.LogoutAsync(
            request.AccessToken,
            cancellationToken);
}
