namespace ProjectBudgetManagement.Domain.Exceptions;

/// <summary>
/// Exception thrown when attempting to create a resource that already exists.
/// </summary>
public class DuplicateResourceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the DuplicateResourceException class.
    /// </summary>
    public DuplicateResourceException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DuplicateResourceException class with inner exception.
    /// </summary>
    public DuplicateResourceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
