namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Response DTO for transaction information.
/// </summary>
public class TransactionResponse
{
    /// <summary>
    /// Gets or sets the transaction ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the transaction amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the transaction date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the transaction classification.
    /// </summary>
    public string Classification { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the digital signature.
    /// </summary>
    public string DigitalSignature { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data hash.
    /// </summary>
    public string DataHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank account ID.
    /// </summary>
    public Guid BankAccountId { get; set; }

    /// <summary>
    /// Gets or sets the accounting account ID.
    /// </summary>
    public Guid AccountingAccountId { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user ID who created the transaction.
    /// </summary>
    public Guid CreatedBy { get; set; }
}
