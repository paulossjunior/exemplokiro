using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Ports;

/// <summary>
/// Repository interface for AccountingAccount entity operations.
/// </summary>
public interface IAccountingAccountRepository
{
    /// <summary>
    /// Gets an accounting account by its ID.
    /// </summary>
    /// <param name="id">The accounting account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The accounting account if found, null otherwise.</returns>
    Task<AccountingAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an accounting account by its identifier.
    /// </summary>
    /// <param name="identifier">The accounting account identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The accounting account if found, null otherwise.</returns>
    Task<AccountingAccount?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all accounting accounts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of accounting accounts.</returns>
    Task<List<AccountingAccount>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new accounting account.
    /// </summary>
    /// <param name="accountingAccount">The accounting account to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(AccountingAccount accountingAccount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
