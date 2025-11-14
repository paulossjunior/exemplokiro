namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Standardized error response format for API errors.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error details.
    /// </summary>
    public ErrorDetails Error { get; set; } = new();
}

/// <summary>
/// Detailed error information.
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed validation errors.
    /// </summary>
    public List<ValidationError>? Details { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the error.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the trace ID for debugging.
    /// </summary>
    public string? TraceId { get; set; }
}

/// <summary>
/// Validation error details.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets or sets the field name that failed validation.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the validation issue description.
    /// </summary>
    public string Issue { get; set; } = string.Empty;
}
