using DressShop.Application.Features.Auth.ForgotPassword;
using DressShop.Application.Features.Auth.Login;
using DressShop.Application.Features.Auth.Logout;
using DressShop.Application.Features.Auth.Refresh;
using DressShop.Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginCommand(
                request.Email,
                request.Password),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RefreshTokenCommand(
                request.RefreshToken),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterCommand(
                request.Email,
                request.Password,
                request.FullName),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        var authorization = Request.Headers.Authorization.ToString();

        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized();
        }

        await sender.Send(
            new LogoutCommand(authorization["Bearer ".Length..].Trim()),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ForgotPasswordCommand(request.Email),
            cancellationToken);

        // Always 204 - never disclose whether the account exists.
        return NoContent();
    }
}

public sealed record LoginRequest(
    string Email,
    string Password);

public sealed record RefreshRequest(
    string RefreshToken);

public sealed record RegisterRequest(
    string Email,
    string Password,
    string? FullName);

public sealed record ForgotPasswordRequest(
    string Email);
