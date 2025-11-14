namespace ProjectBudgetManagement.Api.Exceptions;

/// <summary>
/// Exception thrown when a conflict occurs (e.g., duplicate resource).
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ConflictException class.
    /// </summary>
    public ConflictException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ConflictException class with inner exception.
    /// </summary>
    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
