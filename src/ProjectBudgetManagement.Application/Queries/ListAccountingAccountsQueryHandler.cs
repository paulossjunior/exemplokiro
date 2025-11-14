using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Handler for listing accounting accounts.
/// </summary>
public class ListAccountingAccountsQueryHandler
{
    private readonly IAccountingAccountRepository _repository;

    /// <summary>
    /// Initializes a new instance of the ListAccountingAccountsQueryHandler class.
    /// </summary>
    /// <param name="repository">The accounting account repository.</param>
    public ListAccountingAccountsQueryHandler(IAccountingAccountRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Handles the list accounting accounts query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of accounting accounts.</returns>
    public async Task<List<AccountingAccount>> HandleAsync(
        ListAccountingAccountsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
