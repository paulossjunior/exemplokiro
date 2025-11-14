using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Domain.Entities;

/// <summary>
/// Represents an accounting account for categorizing transactions.
/// </summary>
public class AccountingAccount
{
    /// <summary>
    /// Gets or sets the unique identifier for the accounting account.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the accounting account name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the accounting account identifier (must be unique and follow pattern).
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the accounting account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of transactions associated with this accounting account.
    /// </summary>
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    /// <summary>
    /// Validates the accounting account entity.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Accounting account name is required.");
        }

        if (string.IsNullOrWhiteSpace(Identifier))
        {
            throw new InvalidOperationException("Accounting account identifier is required.");
        }

        // Validate identifier format using AccountIdentifier value object
        try
        {
            _ = new AccountIdentifier(Identifier);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException($"Invalid accounting account identifier: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Determines if this accounting account can be deleted.
    /// </summary>
    /// <returns>True if the account can be deleted, false otherwise.</returns>
    public bool CanBeDeleted()
    {
        return Transactions == null || !Transactions.Any();
    }
}
