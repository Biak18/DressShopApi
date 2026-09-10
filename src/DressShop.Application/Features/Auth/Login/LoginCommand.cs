using DressShop.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace DressShop.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResult>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class LoginCommandHandler(IAuthClient authClient)
    : IRequestHandler<LoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        => authClient.LoginAsync(request.Email, request.Password, cancellationToken);
}
