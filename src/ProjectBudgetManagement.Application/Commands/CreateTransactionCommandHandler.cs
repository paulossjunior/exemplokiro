using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.Services;
using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Handler for CreateTransactionCommand with coordinator validation and digital signature.
/// </summary>
public class CreateTransactionCommandHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IAccountingAccountRepository _accountingAccountRepository;
    private readonly CryptographicService _cryptographicService;
    private readonly DigitalSignatureService _digitalSignatureService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTransactionCommandHandler"/> class.
    /// </summary>
    /// <param name="transactionRepository">The transaction repository.</param>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="accountingAccountRepository">The accounting account repository.</param>
    /// <param name="cryptographicService">The cryptographic service.</param>
    /// <param name="digitalSignatureService">The digital signature service.</param>
    public CreateTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IProjectRepository projectRepository,
        IAccountingAccountRepository accountingAccountRepository,
        CryptographicService cryptographicService,
        DigitalSignatureService digitalSignatureService)
    {
        _transactionRepository = transactionRepository;
        _projectRepository = projectRepository;
        _accountingAccountRepository = accountingAccountRepository;
        _cryptographicService = cryptographicService;
        _digitalSignatureService = digitalSignatureService;
    }

    /// <summary>
    /// Handles the CreateTransactionCommand.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created transaction.</returns>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when coordinator validation fails.</exception>
    public async Task<Transaction> HandleAsync(CreateTransactionCommand command, CancellationToken cancellationToken = default)
    {
        // Validate project exists
        var project = await _projectRepository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project with ID {command.ProjectId} not found.");
        }

        // Validate coordinator authorization - only assigned coordinator can create transactions
        if (project.CoordinatorId != command.UserId)
        {
            throw new UnauthorizedAccessException($"User {command.UserId} is not authorized to create transactions for project {command.ProjectId}. Only the assigned coordinator can create transactions.");
        }

        // Validate project status - prevent transactions on closed projects
        if (!project.CanCreateTransactions())
        {
            throw new InvalidOperationException($"Cannot create transactions for project with status {project.Status}. Transactions are only allowed for projects that are not Completed or Cancelled.");
        }

        // Validate accounting account exists
        var accountingAccount = await _accountingAccountRepository.GetByIdAsync(command.AccountingAccountId, cancellationToken);
        if (accountingAccount == null)
        {
            throw new InvalidOperationException($"Accounting account with ID {command.AccountingAccountId} not found.");
        }

        // Validate bank account exists
        if (project.BankAccount == null)
        {
            throw new InvalidOperationException($"Project {command.ProjectId} does not have an associated bank account.");
        }

        // Create transaction entity
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = command.Amount,
            Date = command.Date,
            Classification = command.Classification,
            BankAccountId = project.BankAccount.Id,
            AccountingAccountId = command.AccountingAccountId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = command.UserId
        };

        // Generate digital signature for non-repudiation
        transaction.DigitalSignature = _digitalSignatureService.GenerateTransactionSignature(
            transaction.Amount,
            transaction.Date,
            (int)transaction.Classification,
            transaction.BankAccountId,
            transaction.AccountingAccountId,
            command.UserId);

        // Compute data hash for integrity verification
        transaction.DataHash = _cryptographicService.ComputeTransactionHash(
            transaction.Amount,
            transaction.Date,
            (int)transaction.Classification,
            transaction.BankAccountId,
            transaction.AccountingAccountId,
            command.UserId);

        // Validate transaction
        transaction.Validate();

        // Add transaction to repository
        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _transactionRepository.SaveChangesAsync(cancellationToken);

        return transaction;
    }
}
