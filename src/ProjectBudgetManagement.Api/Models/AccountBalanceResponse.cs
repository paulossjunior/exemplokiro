namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Response DTO for account balance information.
/// </summary>
public class AccountBalanceResponse
{
    /// <summary>
    /// Gets or sets the bank account ID.
    /// </summary>
    public Guid BankAccountId { get; set; }

    /// <summary>
    /// Gets or sets the current balance.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Gets or sets the total credits.
    /// </summary>
    public decimal TotalCredits { get; set; }

    /// <summary>
    /// Gets or sets the total debits.
    /// </summary>
    public decimal TotalDebits { get; set; }

    /// <summary>
    /// Gets or sets the transaction count.
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// Gets or sets the calculation timestamp.
    /// </summary>
    public DateTime CalculatedAt { get; set; }
}
