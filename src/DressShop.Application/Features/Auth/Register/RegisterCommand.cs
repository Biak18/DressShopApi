using DressShop.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace DressShop.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string? FullName
) : IRequest<RegisterResult>;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FullName).MaximumLength(200);
    }
}

public sealed class RegisterCommandHandler(IAuthClient authClient)
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    public Task<RegisterResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
        => authClient.RegisterAsync(
            request.Email,
            request.Password,
            request.FullName,
            cancellationToken);
}
