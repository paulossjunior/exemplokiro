using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Ports;

/// <summary>
/// Repository interface for Project entity operations.
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Gets a project by its ID.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project if found, null otherwise.</returns>
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all projects with optional filtering.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="skip">Number of records to skip for pagination.</param>
    /// <param name="take">Number of records to take for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of projects.</returns>
    Task<List<Project>> GetAllAsync(
        ProjectStatus? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of projects with optional filtering.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Total count of projects.</returns>
    Task<int> GetCountAsync(ProjectStatus? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new project.
    /// </summary>
    /// <param name="project">The project to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Project project, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="project">The project to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a bank account with the specified combination already exists.
    /// </summary>
    /// <param name="accountNumber">The account number.</param>
    /// <param name="bankName">The bank name.</param>
    /// <param name="branchNumber">The branch number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the bank account exists, false otherwise.</returns>
    Task<bool> BankAccountExistsAsync(
        string accountNumber,
        string bankName,
        string branchNumber,
        CancellationToken cancellationToken = default);
}
