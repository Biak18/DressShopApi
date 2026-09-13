using DressShop.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace DressShop.Application.Features.Auth.Refresh;

public sealed record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthResult>;

public sealed class RefreshTokenCommandValidator
    : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
