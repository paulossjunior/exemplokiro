namespace ProjectBudgetManagement.Api.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the NotFoundException class.
    /// </summary>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the NotFoundException class with inner exception.
    /// </summary>
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
