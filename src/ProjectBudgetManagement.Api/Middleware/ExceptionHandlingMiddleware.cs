using System.Net;
using System.Text.Json;
using ProjectBudgetManagement.Api.Exceptions;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Domain.Exceptions;

namespace ProjectBudgetManagement.Api.Middleware;

/// <summary>
/// Middleware for global exception handling with standardized error responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the ExceptionHandlingMiddleware class.
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
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
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Error = new ErrorDetails
            {
                Timestamp = DateTime.UtcNow,
                TraceId = context.TraceIdentifier
            }
        };

        switch (exception)
        {
            case ValidationException validationEx:
                // Handle validation errors (400)
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Error.Code = "VALIDATION_ERROR";
                errorResponse.Error.Message = validationEx.Message;
                errorResponse.Error.Details = validationEx.Errors
                    .Select(e => new ValidationError { Field = e.Key, Issue = e.Value })
                    .ToList();
                _logger.LogWarning(validationEx, "Validation error: {Message}", validationEx.Message);
                break;

            case UnauthorizedAccessException unauthorizedEx:
                // Handle authorization errors (403)
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                errorResponse.Error.Code = "FORBIDDEN";
                errorResponse.Error.Message = unauthorizedEx.Message;
                _logger.LogWarning(unauthorizedEx, "Authorization error: {Message}", unauthorizedEx.Message);
                break;

            case NotFoundException notFoundEx:
                // Handle not found errors (404)
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Error.Code = "NOT_FOUND";
                errorResponse.Error.Message = notFoundEx.Message;
                _logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
                break;

            case KeyNotFoundException keyNotFoundEx:
                // Handle not found errors (404)
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Error.Code = "NOT_FOUND";
                errorResponse.Error.Message = keyNotFoundEx.Message;
                _logger.LogWarning(keyNotFoundEx, "Resource not found: {Message}", keyNotFoundEx.Message);
                break;

            case ConflictException conflictEx:
                // Handle conflict errors (409)
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Error.Code = "CONFLICT";
                errorResponse.Error.Message = conflictEx.Message;
                _logger.LogWarning(conflictEx, "Conflict error: {Message}", conflictEx.Message);
                break;

            case DuplicateResourceException duplicateEx:
                // Handle duplicate resource errors (409)
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Error.Code = "CONFLICT";
                errorResponse.Error.Message = duplicateEx.Message;
                _logger.LogWarning(duplicateEx, "Duplicate resource error: {Message}", duplicateEx.Message);
                break;

            case IntegrityException integrityEx:
                // Handle integrity errors (500) with security logging
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Error.Code = "INTEGRITY_ERROR";
                errorResponse.Error.Message = "Data integrity verification failed. This incident has been logged.";
                _logger.LogCritical(integrityEx, "SECURITY ALERT - Data integrity violation detected: {Message}", integrityEx.Message);
                break;

            case InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("not found"):
                // Handle not found errors (404)
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Error.Code = "NOT_FOUND";
                errorResponse.Error.Message = invalidOpEx.Message;
                _logger.LogWarning(invalidOpEx, "Resource not found: {Message}", invalidOpEx.Message);
                break;

            case InvalidOperationException invalidOpEx:
                // Handle validation/business rule errors (400)
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Error.Code = "INVALID_OPERATION";
                errorResponse.Error.Message = invalidOpEx.Message;
                _logger.LogWarning(invalidOpEx, "Invalid operation: {Message}", invalidOpEx.Message);
                break;

            case ArgumentException argEx:
                // Handle validation errors (400)
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Error.Code = "INVALID_ARGUMENT";
                errorResponse.Error.Message = argEx.Message;
                _logger.LogWarning(argEx, "Invalid argument: {Message}", argEx.Message);
                break;

            default:
                // Handle unexpected errors (500)
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Error.Code = "INTERNAL_SERVER_ERROR";
                errorResponse.Error.Message = "An unexpected error occurred. Please try again later.";
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var result = JsonSerializer.Serialize(errorResponse, options);
        await response.WriteAsync(result);
    }
}
