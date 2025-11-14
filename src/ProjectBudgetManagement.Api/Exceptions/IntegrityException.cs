namespace ProjectBudgetManagement.Api.Exceptions;

/// <summary>
/// Exception thrown when data integrity verification fails.
/// </summary>
public class IntegrityException : Exception
{
    /// <summary>
    /// Initializes a new instance of the IntegrityException class.
    /// </summary>
    public IntegrityException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the IntegrityException class with inner exception.
    /// </summary>
    public IntegrityException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
