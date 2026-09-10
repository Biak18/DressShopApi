using DressShop.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Middleware;

/// <summary>
/// Single place where unhandled exceptions become HTTP responses.
/// This is what keeps controllers free of try/catch (ARCHITECTURE.md rule 10).
///
/// Anything not explicitly mapped below falls through to a generic 500 with
/// no exception detail in the body - stack traces/messages go to the logs
/// (via ILogger), never to the client. See ARCHITECTURE.md section 49.
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                string.Join(" ", validationException.Errors.Select(e => e.ErrorMessage))),

            DomainException domainException => (
                StatusCodes.Status400BadRequest,
                "Business rule violation",
                domainException.Message),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                "The requested resource could not be found."),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Authentication failed",
                exception.Message),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                "The server encountered an unexpected condition. Please try again later.")

        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception ({StatusCode}) processing {Method} {Path}",
                statusCode, httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        }, cancellationToken);

        return true;
    }
}
