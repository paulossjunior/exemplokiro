using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Domain.Services;

/// <summary>
/// Service for calculating account balances and comparing against budgets.
/// </summary>
public class BalanceCalculationService
{
    /// <summary>
    /// Calculates the current balance from transaction history.
    /// </summary>
    /// <param name="transactions">The list of transactions.</param>
    /// <returns>The calculated balance.</returns>
    public decimal CalculateBalance(IEnumerable<Transaction> transactions)
    {
        if (transactions == null || !transactions.Any())
        {
            return 0m;
        }

        decimal balance = 0m;

        foreach (var transaction in transactions.OrderBy(t => t.Date).ThenBy(t => t.CreatedAt))
        {
            if (transaction.Classification == TransactionClassification.Credit)
            {
                balance += transaction.Amount;
            }
            else if (transaction.Classification == TransactionClassification.Debit)
            {
                balance -= transaction.Amount;
            }
        }

        return balance;
    }

    /// <summary>
    /// Compares the current balance against the project budget.
    /// </summary>
    /// <param name="balance">The current balance.</param>
    /// <param name="budget">The project budget.</param>
    /// <returns>True if balance exceeds budget, false otherwise.</returns>
    public bool IsBalanceOverBudget(decimal balance, decimal budget)
    {
        return balance > budget;
    }

    /// <summary>
    /// Generates a balance warning if the balance exceeds the budget.
    /// </summary>
    /// <param name="balance">The current balance.</param>
    /// <param name="budget">The project budget.</param>
    /// <returns>A warning message if balance exceeds budget, null otherwise.</returns>
    public string? GenerateBalanceWarning(decimal balance, decimal budget)
    {
        if (IsBalanceOverBudget(balance, budget))
        {
            var overage = balance - budget;
            return $"Warning: Account balance ({balance:F2}) exceeds project budget ({budget:F2}) by {overage:F2}";
        }

        return null;
    }

    /// <summary>
    /// Calculates running balance for each transaction in chronological order.
    /// </summary>
    /// <param name="transactions">The list of transactions.</param>
    /// <returns>A dictionary mapping transaction IDs to their running balance.</returns>
    public Dictionary<Guid, decimal> CalculateRunningBalances(IEnumerable<Transaction> transactions)
    {
        var runningBalances = new Dictionary<Guid, decimal>();
        decimal currentBalance = 0m;

        foreach (var transaction in transactions.OrderBy(t => t.Date).ThenBy(t => t.CreatedAt))
        {
            if (transaction.Classification == TransactionClassification.Credit)
            {
                currentBalance += transaction.Amount;
            }
            else if (transaction.Classification == TransactionClassification.Debit)
            {
                currentBalance -= transaction.Amount;
            }

            runningBalances[transaction.Id] = currentBalance;
        }

        return runningBalances;
    }
}
