namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Request DTO for creating a new transaction.
/// </summary>
public class CreateTransactionRequest
{
    /// <summary>
    /// Gets or sets the transaction amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the transaction date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the transaction classification (Debit or Credit).
    /// </summary>
    public string Classification { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the accounting account ID.
    /// </summary>
    public Guid AccountingAccountId { get; set; }
}
