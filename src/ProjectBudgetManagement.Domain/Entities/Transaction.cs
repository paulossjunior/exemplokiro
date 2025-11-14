using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Domain.Entities;

/// <summary>
/// Represents an immutable financial transaction on a bank account.
/// </summary>
public class Transaction
{
    /// <summary>
    /// Gets or sets the unique identifier for the transaction.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the transaction amount (must be positive).
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the transaction date (cannot be in the future).
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the transaction classification (Debit or Credit).
    /// </summary>
    public TransactionClassification Classification { get; set; }

    /// <summary>
    /// Gets or sets the digital signature linking transaction to coordinator.
    /// </summary>
    public string DigitalSignature { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cryptographic hash for data integrity verification.
    /// </summary>
    public string DataHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank account ID this transaction belongs to.
    /// </summary>
    public Guid BankAccountId { get; set; }

    /// <summary>
    /// Gets or sets the bank account this transaction belongs to.
    /// </summary>
    public virtual BankAccount? BankAccount { get; set; }

    /// <summary>
    /// Gets or sets the accounting account ID for categorization.
    /// </summary>
    public Guid AccountingAccountId { get; set; }

    /// <summary>
    /// Gets or sets the accounting account for categorization.
    /// </summary>
    public virtual AccountingAccount? AccountingAccount { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transaction was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user ID who created the transaction.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Validates the transaction entity.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (Amount <= 0)
        {
            throw new InvalidOperationException("Transaction amount must be greater than zero.");
        }

        if (Date > DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException("Transaction date cannot be in the future.");
        }

        if (string.IsNullOrWhiteSpace(DigitalSignature))
        {
            throw new InvalidOperationException("Transaction must have a digital signature.");
        }

        if (string.IsNullOrWhiteSpace(DataHash))
        {
            throw new InvalidOperationException("Transaction must have a data hash.");
        }
    }
}
