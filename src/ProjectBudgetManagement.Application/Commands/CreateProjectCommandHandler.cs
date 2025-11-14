using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.Exceptions;
using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Handler for CreateProjectCommand.
/// </summary>
public class CreateProjectCommandHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPersonRepository _personRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="personRepository">The person repository.</param>
    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IPersonRepository personRepository)
    {
        _projectRepository = projectRepository;
        _personRepository = personRepository;
    }

    /// <summary>
    /// Handles the CreateProjectCommand.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created project.</returns>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public async Task<Project> HandleAsync(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        // Validate coordinator exists
        var coordinator = await _personRepository.GetByIdAsync(command.CoordinatorId, cancellationToken);
        if (coordinator == null)
        {
            throw new InvalidOperationException($"Coordinator with ID {command.CoordinatorId} not found.");
        }

        // Create project entity
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Status = ProjectStatus.NotStarted, // Default status as per requirement 1.5
            BudgetAmount = command.BudgetAmount,
            CoordinatorId = command.CoordinatorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Validate project
        project.Validate();

        // Check for duplicate bank account
        var bankAccountExists = await _projectRepository.BankAccountExistsAsync(
            command.AccountNumber,
            command.BankName,
            command.BranchNumber,
            cancellationToken);

        if (bankAccountExists)
        {
            throw new DuplicateResourceException(
                $"A bank account with account number '{command.AccountNumber}', " +
                $"bank '{command.BankName}', and branch '{command.BranchNumber}' already exists.");
        }

        // Create bank account for the project
        var bankAccount = new BankAccount
        {
            Id = Guid.NewGuid(),
            AccountNumber = command.AccountNumber,
            BankName = command.BankName,
            BranchNumber = command.BranchNumber,
            AccountHolderName = command.AccountHolderName,
            ProjectId = project.Id,
            CreatedAt = DateTime.UtcNow
        };

        // Validate bank account
        bankAccount.Validate();

        // Set bank account on project
        project.BankAccount = bankAccount;

        // Add project to repository
        await _projectRepository.AddAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return project;
    }
}
