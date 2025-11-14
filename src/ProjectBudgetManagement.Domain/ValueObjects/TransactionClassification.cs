namespace ProjectBudgetManagement.Domain.ValueObjects;

/// <summary>
/// Represents the classification of a financial transaction.
/// </summary>
public enum TransactionClassification
{
    /// <summary>
    /// A debit transaction (money going out).
    /// </summary>
    Debit = 0,

    /// <summary>
    /// A credit transaction (money coming in).
    /// </summary>
    Credit = 1
}
