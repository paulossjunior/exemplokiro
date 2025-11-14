using ProjectBudgetManagement.Application.Ports;

namespace ProjectBudgetManagement.Application.Services;

/// <summary>
/// Service for authorization checks and access control.
/// </summary>
public class AuthorizationService
{
    private readonly IProjectRepository _projectRepository;

    /// <summary>
    /// Initializes a new instance of the AuthorizationService class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    public AuthorizationService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    /// <summary>
    /// Validates that a user is the coordinator of a project.
    /// </summary>
    /// <param name="userId">The user ID to validate.</param>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if user is the coordinator, false otherwise.</returns>
    public async Task<bool> IsProjectCoordinatorAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        return project != null && project.CoordinatorId == userId;
    }

    /// <summary>
    /// Validates that a user can create transactions for a bank account.
    /// </summary>
    /// <param name="userId">The user ID to validate.</param>
    /// <param name="bankAccountId">The bank account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if user is authorized, false otherwise.</returns>
    public async Task<bool> CanCreateTransactionForBankAccountAsync(
        Guid userId,
        Guid bankAccountId,
        CancellationToken cancellationToken = default)
    {
        // Get all projects and find the one with matching bank account
        var projects = await _projectRepository.GetAllAsync(
            status: null,
            skip: 0,
            take: int.MaxValue,
            cancellationToken);

        var project = projects.FirstOrDefault(p => p.BankAccount?.Id == bankAccountId);
        return project != null && project.CoordinatorId == userId;
    }

    /// <summary>
    /// Ensures that a user is the coordinator of a project, throwing an exception if not.
    /// </summary>
    /// <param name="userId">The user ID to validate.</param>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when user is not authorized.</exception>
    public async Task EnsureIsProjectCoordinatorAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var isCoordinator = await IsProjectCoordinatorAsync(userId, projectId, cancellationToken);
        if (!isCoordinator)
        {
            throw new UnauthorizedAccessException(
                $"User {userId} is not authorized to access project {projectId}. " +
                $"Only the project coordinator can perform this action.");
        }
    }

    /// <summary>
    /// Ensures that a user can create transactions for a bank account, throwing an exception if not.
    /// </summary>
    /// <param name="userId">The user ID to validate.</param>
    /// <param name="bankAccountId">The bank account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when user is not authorized.</exception>
    public async Task EnsureCanCreateTransactionForBankAccountAsync(
        Guid userId,
        Guid bankAccountId,
        CancellationToken cancellationToken = default)
    {
        var canCreate = await CanCreateTransactionForBankAccountAsync(userId, bankAccountId, cancellationToken);
        if (!canCreate)
        {
            throw new UnauthorizedAccessException(
                $"User {userId} is not authorized to create transactions for bank account {bankAccountId}. " +
                $"Only the project coordinator can create transactions.");
        }
    }

    /// <summary>
    /// Validates that a user has access to audit trail data.
    /// For now, all authenticated users can access audit trail.
    /// This can be extended with role-based access control.
    /// </summary>
    /// <param name="userId">The user ID to validate.</param>
    /// <returns>True if user has access, false otherwise.</returns>
    public bool CanAccessAuditTrail(Guid userId)
    {
        // For now, all authenticated users can access audit trail
        // This can be extended with role-based access control
        return userId != Guid.Empty;
    }

    /// <summary>
    /// Ensures that a user has access to audit trail data, throwing an exception if not.
    /// </summary>
    /// <param name="userId">The user ID to validate.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when user is not authorized.</exception>
    public void EnsureCanAccessAuditTrail(Guid userId)
    {
        if (!CanAccessAuditTrail(userId))
        {
            throw new UnauthorizedAccessException(
                $"User {userId} is not authorized to access audit trail data.");
        }
    }
}
