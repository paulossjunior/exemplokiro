using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.Services;
using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Services;

/// <summary>
/// Service for managing transactions with digital signatures and audit logging.
/// </summary>
public class TransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly CryptographicService _cryptographicService;
    private readonly DigitalSignatureService _digitalSignatureService;
    private readonly AuditService _auditService;

    /// <summary>
    /// Initializes a new instance of the TransactionService class.
    /// </summary>
    /// <param name="transactionRepository">The transaction repository.</param>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="cryptographicService">The cryptographic service.</param>
    /// <param name="digitalSignatureService">The digital signature service.</param>
    /// <param name="auditService">The audit service.</param>
    public TransactionService(
        ITransactionRepository transactionRepository,
        IProjectRepository projectRepository,
        CryptographicService cryptographicService,
        DigitalSignatureService digitalSignatureService,
        AuditService auditService)
    {
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _cryptographicService = cryptographicService ?? throw new ArgumentNullException(nameof(cryptographicService));
        _digitalSignatureService = digitalSignatureService ?? throw new ArgumentNullException(nameof(digitalSignatureService));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
    }

    /// <summary>
    /// Creates a new transaction with digital signature and audit logging.
    /// </summary>
    /// <param name="transaction">The transaction to create.</param>
    /// <param name="userId">The authenticated user ID (must be project coordinator).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created transaction.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user is not authorized.</exception>
    /// <exception cref="InvalidOperationException">Thrown when business rules are violated.</exception>
    public async Task<Transaction> CreateTransactionAsync(
        Transaction transaction,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        // Get the project associated with the bank account
        var project = await GetProjectByBankAccountIdAsync(transaction.BankAccountId, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"No project found for bank account ID {transaction.BankAccountId}.");
        }

        // Validate coordinator authorization
        if (project.CoordinatorId != userId)
        {
            throw new UnauthorizedAccessException(
                $"User {userId} is not authorized to create transactions for project {project.Id}. " +
                $"Only the project coordinator can create transactions.");
        }

        // Validate project status - cannot create transactions on closed projects
        if (!project.CanCreateTransactions())
        {
            throw new InvalidOperationException(
                $"Cannot create transactions for project {project.Id} with status {project.Status}. " +
                $"Transactions can only be created for projects that are not Completed or Cancelled.");
        }

        // Set transaction metadata
        transaction.CreatedAt = DateTime.UtcNow;
        transaction.CreatedBy = userId;

        // Generate digital signature for non-repudiation
        transaction.DigitalSignature = _digitalSignatureService.GenerateTransactionSignature(
            transaction.Amount,
            transaction.Date,
            (int)transaction.Classification,
            transaction.BankAccountId,
            transaction.AccountingAccountId,
            userId);

        // Compute cryptographic hash for integrity verification
        transaction.DataHash = _cryptographicService.ComputeTransactionHash(
            transaction.Amount,
            transaction.Date,
            (int)transaction.Classification,
            transaction.BankAccountId,
            transaction.AccountingAccountId,
            userId);

        // Validate transaction
        transaction.Validate();

        // Save transaction
        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _transactionRepository.SaveChangesAsync(cancellationToken);

        // Log transaction creation in audit trail
        await _auditService.LogTransactionAsync(userId, transaction, cancellationToken);

        return transaction;
    }

    /// <summary>
    /// Gets transaction history for a bank account with optional filtering.
    /// </summary>
    /// <param name="bankAccountId">The bank account ID.</param>
    /// <param name="startDate">Optional start date filter.</param>
    /// <param name="endDate">Optional end date filter.</param>
    /// <param name="classification">Optional classification filter.</param>
    /// <param name="accountingAccountId">Optional accounting account filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of transactions.</returns>
    public async Task<List<Transaction>> GetTransactionHistoryAsync(
        Guid bankAccountId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        TransactionClassification? classification = null,
        Guid? accountingAccountId = null,
        CancellationToken cancellationToken = default)
    {
        return await _transactionRepository.GetByBankAccountAsync(
            bankAccountId,
            startDate,
            endDate,
            classification,
            accountingAccountId,
            cancellationToken);
    }

    /// <summary>
    /// Gets a transaction by ID.
    /// </summary>
    /// <param name="transactionId">The transaction ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The transaction if found, null otherwise.</returns>
    public async Task<Transaction?> GetTransactionByIdAsync(
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        return await _transactionRepository.GetByIdAsync(transactionId, cancellationToken);
    }

    /// <summary>
    /// Validates that a user is authorized to create transactions for a bank account.
    /// </summary>
    /// <param name="bankAccountId">The bank account ID.</param>
    /// <param name="userId">The user ID to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if authorized, false otherwise.</returns>
    public async Task<bool> IsUserAuthorizedForBankAccountAsync(
        Guid bankAccountId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var project = await GetProjectByBankAccountIdAsync(bankAccountId, cancellationToken);
        return project != null && project.CoordinatorId == userId;
    }

    /// <summary>
    /// Gets the project associated with a bank account.
    /// </summary>
    private async Task<Project?> GetProjectByBankAccountIdAsync(
        Guid bankAccountId,
        CancellationToken cancellationToken)
    {
        // Get all projects and find the one with matching bank account
        // This is a simplified approach - in production, you might want a more efficient query
        var projects = await _projectRepository.GetAllAsync(
            status: null,
            skip: 0,
            take: int.MaxValue,
            cancellationToken);

        return projects.FirstOrDefault(p => p.BankAccount?.Id == bankAccountId);
    }
}
