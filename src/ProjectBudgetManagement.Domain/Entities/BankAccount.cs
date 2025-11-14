namespace ProjectBudgetManagement.Domain.Entities;

/// <summary>
/// Represents a bank account associated with a project.
/// </summary>
public class BankAccount
{
    /// <summary>
    /// Gets or sets the unique identifier for the bank account.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the account number (must be numeric).
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank name.
    /// </summary>
    public string BankName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch number (must be numeric).
    /// </summary>
    public string BranchNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the account holder name (should match project coordinator).
    /// </summary>
    public string AccountHolderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project ID this bank account belongs to.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the project this bank account belongs to.
    /// </summary>
    public virtual Project? Project { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the bank account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of transactions for this bank account.
    /// </summary>
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    /// <summary>
    /// Validates the bank account entity.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccountNumber))
        {
            throw new InvalidOperationException("Account number is required.");
        }

        if (!IsNumeric(AccountNumber))
        {
            throw new InvalidOperationException("Account number must contain only numeric characters.");
        }

        if (string.IsNullOrWhiteSpace(BankName))
        {
            throw new InvalidOperationException("Bank name is required.");
        }

        if (string.IsNullOrWhiteSpace(BranchNumber))
        {
            throw new InvalidOperationException("Branch number is required.");
        }

        if (!IsNumeric(BranchNumber))
        {
            throw new InvalidOperationException("Branch number must contain only numeric characters.");
        }

        if (string.IsNullOrWhiteSpace(AccountHolderName))
        {
            throw new InvalidOperationException("Account holder name is required.");
        }
    }

    private static bool IsNumeric(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && value.All(char.IsDigit);
    }
}
