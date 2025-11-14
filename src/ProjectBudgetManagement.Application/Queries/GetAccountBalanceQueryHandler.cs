using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Services;

namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Handler for GetAccountBalanceQuery with budget comparison.
/// </summary>
public class GetAccountBalanceQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly BalanceCalculationService _balanceCalculationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAccountBalanceQueryHandler"/> class.
    /// </summary>
    /// <param name="transactionRepository">The transaction repository.</param>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="balanceCalculationService">The balance calculation service.</param>
    public GetAccountBalanceQueryHandler(
        ITransactionRepository transactionRepository,
        IProjectRepository projectRepository,
        BalanceCalculationService balanceCalculationService)
    {
        _transactionRepository = transactionRepository;
        _projectRepository = projectRepository;
        _balanceCalculationService = balanceCalculationService;
    }

    /// <summary>
    /// Handles the GetAccountBalanceQuery.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Account balance result with budget comparison.</returns>
    /// <exception cref="InvalidOperationException">Thrown when project not found.</exception>
    public async Task<AccountBalanceResult> HandleAsync(GetAccountBalanceQuery query, CancellationToken cancellationToken = default)
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

        // Get all transactions for the bank account
        var transactions = await _transactionRepository.GetByBankAccountAsync(
            project.BankAccount.Id,
            cancellationToken: cancellationToken);

        // Calculate current balance
        var currentBalance = _balanceCalculationService.CalculateBalance(transactions);

        // Check if balance exceeds budget
        var isOverBudget = _balanceCalculationService.IsBalanceOverBudget(currentBalance, project.BudgetAmount);

        // Generate warning if over budget
        var warning = _balanceCalculationService.GenerateBalanceWarning(currentBalance, project.BudgetAmount);

        return new AccountBalanceResult
        {
            ProjectId = query.ProjectId,
            CurrentBalance = Math.Round(currentBalance, 2),
            BudgetAmount = Math.Round(project.BudgetAmount, 2),
            IsOverBudget = isOverBudget,
            Warning = warning
        };
    }
}

/// <summary>
/// Result containing account balance information with budget comparison.
/// </summary>
public class AccountBalanceResult
{
    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the current balance (with two decimal precision).
    /// </summary>
    public decimal CurrentBalance { get; set; }

    /// <summary>
    /// Gets or sets the project budget amount (with two decimal precision).
    /// </summary>
    public decimal BudgetAmount { get; set; }

    /// <summary>
    /// Gets or sets whether the balance exceeds the budget.
    /// </summary>
    public bool IsOverBudget { get; set; }

    /// <summary>
    /// Gets or sets the warning message if balance exceeds budget.
    /// </summary>
    public string? Warning { get; set; }
}
