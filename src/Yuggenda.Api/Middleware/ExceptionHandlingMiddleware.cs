using Yuggenda.Application.Exceptions;
using Yuggenda.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Yuggenda.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status404NotFound,
                "Not Found",
                exception.Message);
        }
        catch (ConflictException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message);
        }
        catch (UnauthorizedAccessException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                exception.Message);
        }
        catch (ArgumentException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Bad Request",
                exception.Message);
        }
        catch (DomainException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unexpected error occurred while processing {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.");
        }
    }

    private async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        var problem = new ProblemDetails
        {
            Type = "about:blank",
            Title = title,
            Status = statusCode,
            Detail = detail
        };

        problem.Extensions["requestId"] = context.TraceIdentifier;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }
}