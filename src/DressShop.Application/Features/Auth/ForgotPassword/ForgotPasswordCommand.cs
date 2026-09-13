using DressShop.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace DressShop.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string Email
) : IRequest;

public sealed class ForgotPasswordCommandValidator
    : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public sealed class ForgotPasswordCommandHandler(IAuthClient authClient)
    : IRequestHandler<ForgotPasswordCommand>
{
    public Task Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
        => authClient.RequestPasswordResetAsync(
            request.Email,
            cancellationToken);
}
