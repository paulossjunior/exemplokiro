using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Ports;

/// <summary>
/// Repository interface for Person entity operations.
/// </summary>
public interface IPersonRepository
{
    /// <summary>
    /// Gets a person by their ID.
    /// </summary>
    /// <param name="id">The person ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The person if found, null otherwise.</returns>
    Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a person by their identification number.
    /// </summary>
    /// <param name="identificationNumber">The identification number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The person if found, null otherwise.</returns>
    Task<Person?> GetByIdentificationNumberAsync(string identificationNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all persons.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of persons.</returns>
    Task<List<Person>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new person.
    /// </summary>
    /// <param name="person">The person to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Person person, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
