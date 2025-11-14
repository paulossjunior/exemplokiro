using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.Services;

namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Handler for GetTransactionHistoryQuery with filtering and running balance calculation.
/// </summary>
public class GetTransactionHistoryQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly BalanceCalculationService _balanceCalculationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTransactionHistoryQueryHandler"/> class.
    /// </summary>
    /// <param name="transactionRepository">The transaction repository.</param>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="balanceCalculationService">The balance calculation service.</param>
    public GetTransactionHistoryQueryHandler(
        ITransactionRepository transactionRepository,
        IProjectRepository projectRepository,
        BalanceCalculationService balanceCalculationService)
    {
        _transactionRepository = transactionRepository;
        _projectRepository = projectRepository;
        _balanceCalculationService = balanceCalculationService;
    }

    /// <summary>
    /// Handles the GetTransactionHistoryQuery.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction history result with running balances.</returns>
    /// <exception cref="InvalidOperationException">Thrown when project not found.</exception>
    public async Task<TransactionHistoryResult> HandleAsync(GetTransactionHistoryQuery query, CancellationToken cancellationToken = default)
    {
        // Validate project exists
        var project = await _projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project with ID {query.ProjectId} not found.");
        }

        // Validate bank account exists
        if (project.BankAccount == null)
        {
            throw new InvalidOperationException($"Project {query.ProjectId} does not have an associated bank account.");
        }

        // Get transactions with filters
        var transactions = await _transactionRepository.GetByBankAccountAsync(
            project.BankAccount.Id,
            query.StartDate,
            query.EndDate,
            query.Classification,
            query.AccountingAccountId,
            cancellationToken);

        // Calculate running balances
        var runningBalances = _balanceCalculationService.CalculateRunningBalances(transactions);

        return new TransactionHistoryResult
        {
            ProjectId = query.ProjectId,
            Transactions = transactions,
            RunningBalances = runningBalances
        };
    }
}

/// <summary>
/// Result containing transaction history with running balances.
/// </summary>
public class TransactionHistoryResult
{
    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the list of transactions in chronological order.
    /// </summary>
    public List<Transaction> Transactions { get; set; } = new();

    /// <summary>
    /// Gets or sets the running balance for each transaction.
    /// </summary>
    public Dictionary<Guid, decimal> RunningBalances { get; set; } = new();
}
