namespace ProjectBudgetManagement.Api.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public Dictionary<string, string> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationException class.
    /// </summary>
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string>();
    }

    /// <summary>
    /// Initializes a new instance of the ValidationException class with validation errors.
    /// </summary>
    public ValidationException(string message, Dictionary<string, string> errors) : base(message)
    {
        Errors = errors;
    }
}
