using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WexChallenge.Domain.Exceptions;

namespace WexChallenge.Middleware;

/// <summary>
/// Global exception handler middleware that converts exceptions to ProblemDetails (RFC 7807).
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            PurchaseNotFoundException ex => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Resource Not Found",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.NotFound,
                Extensions = { { "code", ex.Code }, { "purchaseId", ex.PurchaseId } }
            },
            ExchangeRateUnavailableException ex => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Currency Conversion Unavailable",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.UnprocessableEntity,
                Extensions = { { "code", ex.Code }, { "currency", ex.Currency }, { "purchaseDate", ex.PurchaseDate.ToString("yyyy-MM-dd") } }
            },
            DomainException ex => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Domain Error",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Extensions = { { "code", ex.Code } }
            },
            HttpRequestException ex => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "External Service Error",
                Detail = "An error occurred while communicating with an external service. Please try again later.",
                Status = (int)HttpStatusCode.ServiceUnavailable
            },
            ArgumentException ex => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Invalid Argument",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest
            },
            _ => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please try again later.",
                Status = (int)HttpStatusCode.InternalServerError
            }
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}

/// <summary>
/// Extension methods for adding the global exception handler middleware.
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
