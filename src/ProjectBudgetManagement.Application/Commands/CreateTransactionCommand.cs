using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Command to create a new transaction with authentication requirement.
/// </summary>
public class CreateTransactionCommand
{
    /// <summary>
    /// Gets or sets the project ID for which the transaction is being created.
    /// </summary>
    public Guid ProjectId { get; set; }

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
    /// Gets or sets the accounting account ID for categorization.
    /// </summary>
    public Guid AccountingAccountId { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user ID (coordinator).
    /// </summary>
    public Guid UserId { get; set; }
}
