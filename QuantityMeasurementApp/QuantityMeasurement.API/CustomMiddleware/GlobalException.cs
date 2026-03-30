using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuantityMeasurement.API.Middleware;

public class GlobalException
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalException> _logger;

    public GlobalException(RequestDelegate next, ILogger<GlobalException> logger)
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
            if (context.Response.HasStarted)
            {
                _logger.LogWarning(ex, "Response has already started. Global exception response was skipped.");
                throw;
            }

            var (status, title, detail, errors) = MapException(ex);

            _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            if (errors.Count > 0)
            {
                problem.Extensions["errors"] = errors;
            }

            var payload = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(payload);
        }
    }

    private static (int status, string title, string detail, Dictionary<string, string[]> errors) MapException(Exception ex)
    {
        var errors = new Dictionary<string, string[]>();

        switch (ex)
        {
            case BadHttpRequestException badRequestEx:
                return (StatusCodes.Status400BadRequest, "Bad Request", badRequestEx.Message, errors);

            case JsonException jsonEx:
            {
                var path = string.IsNullOrWhiteSpace(jsonEx.Path) ? "requestBody" : jsonEx.Path!;
                errors[path] = new[]
                {
                    "Invalid JSON format, invalid value type, or malformed request body."
                };
                return (
                    StatusCodes.Status400BadRequest,
                    "Invalid Request Body",
                    "Request body contains invalid JSON or invalid field values.",
                    errors
                );
            }

            case ArgumentNullException argNullEx:
            {
                var field = string.IsNullOrWhiteSpace(argNullEx.ParamName) ? "input" : argNullEx.ParamName!;
                errors[field] = new[] { "This field is required and was not provided." };
                return (
                    StatusCodes.Status400BadRequest,
                    "Missing Required Input",
                    argNullEx.Message,
                    errors
                );
            }

            case ArgumentException argEx:
            {
                var field = string.IsNullOrWhiteSpace(argEx.ParamName) ? "input" : argEx.ParamName!;
                errors[field] = new[] { argEx.Message };
                return (
                    StatusCodes.Status400BadRequest,
                    "Invalid Input",
                    "One or more input values are invalid.",
                    errors
                );
            }

            case FormatException fmtEx:
                return (
                    StatusCodes.Status400BadRequest,
                    "Invalid Format",
                    fmtEx.Message,
                    errors
                );

            case DivideByZeroException divideEx:
                return (
                    StatusCodes.Status400BadRequest,
                    "Invalid Mathematical Operation",
                    divideEx.Message,
                    errors
                );

            case UnauthorizedAccessException unauthEx:
                return (
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    unauthEx.Message,
                    errors
                );

            case KeyNotFoundException notFoundEx:
                return (
                    StatusCodes.Status404NotFound,
                    "Not Found",
                    notFoundEx.Message,
                    errors
                );

            case InvalidOperationException invalidOpEx:
            {
                if (invalidOpEx.Message.Contains("Nullable object must have a value", StringComparison.OrdinalIgnoreCase))
                {
                    errors["input"] = new[]
                    {
                        "A required numeric input is missing (for example Value2 or Scalar, depending on the endpoint)."
                    };

                    return (
                        StatusCodes.Status400BadRequest,
                        "Missing Required Input",
                        "One or more required numeric fields were missing.",
                        errors
                    );
                }

                return (
                    StatusCodes.Status409Conflict,
                    "Invalid Operation",
                    invalidOpEx.Message,
                    errors
                );
            }

            case DbUpdateConcurrencyException concurrencyEx:
                return (
                    StatusCodes.Status409Conflict,
                    "Concurrency Conflict",
                    concurrencyEx.Message,
                    errors
                );

            case DbUpdateException dbUpdateEx:
                return (
                    StatusCodes.Status500InternalServerError,
                    "Database Update Failed",
                    dbUpdateEx.Message,
                    errors
                );

            case TimeoutException timeoutEx:
                return (
                    StatusCodes.Status504GatewayTimeout,
                    "Request Timeout",
                    timeoutEx.Message,
                    errors
                );

            case NotImplementedException notImplementedEx:
                return (
                    StatusCodes.Status501NotImplemented,
                    "Not Implemented",
                    notImplementedEx.Message,
                    errors
                );

            default:
                return (
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred. Please try again later.",
                    errors
                );
        }
    }
}

public static class GlobalExceptionExtensions
{
    public static IApplicationBuilder UseGlobalException(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalException>();
    }
}