using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Handler for creating accounting accounts.
/// </summary>
public class CreateAccountingAccountCommandHandler
{
    private readonly IAccountingAccountRepository _repository;

    /// <summary>
    /// Initializes a new instance of the CreateAccountingAccountCommandHandler class.
    /// </summary>
    /// <param name="repository">The accounting account repository.</param>
    public CreateAccountingAccountCommandHandler(IAccountingAccountRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Handles the create accounting account command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created accounting account.</returns>
    /// <exception cref="InvalidOperationException">Thrown when identifier already exists.</exception>
    public async Task<AccountingAccount> HandleAsync(
        CreateAccountingAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        // Check if identifier already exists
        var existing = await _repository.GetByIdentifierAsync(command.Identifier, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException(
                $"An accounting account with identifier '{command.Identifier}' already exists.");
        }

        var accountingAccount = new AccountingAccount
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Identifier = command.Identifier,
            CreatedAt = DateTime.UtcNow
        };

        // Validate identifier format and business rules
        accountingAccount.Validate();

        await _repository.AddAsync(accountingAccount, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return accountingAccount;
    }
}
