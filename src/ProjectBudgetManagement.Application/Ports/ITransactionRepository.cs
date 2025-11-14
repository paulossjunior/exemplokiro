using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Ports;

/// <summary>
/// Repository interface for Transaction entity operations.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Gets a transaction by its ID.
    /// </summary>
    /// <param name="id">The transaction ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The transaction if found, null otherwise.</returns>
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all transactions for a bank account with optional filtering.
    /// </summary>
    /// <param name="bankAccountId">The bank account ID.</param>
    /// <param name="startDate">Optional start date filter.</param>
    /// <param name="endDate">Optional end date filter.</param>
    /// <param name="classification">Optional classification filter.</param>
    /// <param name="accountingAccountId">Optional accounting account filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of transactions.</returns>
    Task<List<Transaction>> GetByBankAccountAsync(
        Guid bankAccountId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        TransactionClassification? classification = null,
        Guid? accountingAccountId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new transaction.
    /// </summary>
    /// <param name="transaction">The transaction to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
