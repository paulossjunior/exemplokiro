using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Services;

namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Handler for verifying data integrity.
/// </summary>
public class VerifyDataIntegrityQueryHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IntegrityVerificationService _integrityVerificationService;

    /// <summary>
    /// Initializes a new instance of the VerifyDataIntegrityQueryHandler class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="transactionRepository">The transaction repository.</param>
    /// <param name="auditRepository">The audit repository.</param>
    /// <param name="integrityVerificationService">The integrity verification service.</param>
    public VerifyDataIntegrityQueryHandler(
        IProjectRepository projectRepository,
        ITransactionRepository transactionRepository,
        IAuditRepository auditRepository,
        IntegrityVerificationService integrityVerificationService)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        _integrityVerificationService = integrityVerificationService ?? throw new ArgumentNullException(nameof(integrityVerificationService));
    }

    /// <summary>
    /// Handles the verify data integrity query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The integrity report.</returns>
    /// <exception cref="InvalidOperationException">Thrown when project is not found.</exception>
    public async Task<IntegrityReport> HandleAsync(
        VerifyDataIntegrityQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        // Get project to verify it exists
        var project = await _projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project with ID {query.ProjectId} not found.");
        }

        if (project.BankAccount == null)
        {
            throw new InvalidOperationException($"Project {query.ProjectId} does not have an associated bank account.");
        }

        // Get all transactions for the project's bank account
        var transactions = await _transactionRepository.GetByBankAccountAsync(
            project.BankAccount.Id,
            cancellationToken: cancellationToken);

        // Get all audit entries for the project
        var auditEntries = await _auditRepository.GetAuditTrailAsync(
            entityId: query.ProjectId,
            cancellationToken: cancellationToken);

        // Generate integrity report
        return _integrityVerificationService.GenerateIntegrityReport(
            transactions,
            auditEntries);
    }
}
