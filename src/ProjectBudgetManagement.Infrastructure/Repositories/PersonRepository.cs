using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Infrastructure.Persistence;

namespace ProjectBudgetManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Person entity.
/// </summary>
public class PersonRepository : IPersonRepository
{
    private readonly ProjectBudgetDbContext _context;

    // Compiled query for getting person by ID
    private static readonly Func<ProjectBudgetDbContext, Guid, Task<Person?>> GetPersonByIdCompiled =
        EF.CompileAsyncQuery((ProjectBudgetDbContext context, Guid id) =>
            context.Persons.FirstOrDefault(p => p.Id == id));

    // Compiled query for getting person by identification number
    private static readonly Func<ProjectBudgetDbContext, string, Task<Person?>> GetPersonByIdentificationCompiled =
        EF.CompileAsyncQuery((ProjectBudgetDbContext context, string identificationNumber) =>
            context.Persons.FirstOrDefault(p => p.IdentificationNumber == identificationNumber));

    /// <summary>
    /// Initializes a new instance of the PersonRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public PersonRepository(ProjectBudgetDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetPersonByIdCompiled(_context, id);
    }

    /// <inheritdoc />
    public async Task<Person?> GetByIdentificationNumberAsync(string identificationNumber, CancellationToken cancellationToken = default)
    {
        return await GetPersonByIdentificationCompiled(_context, identificationNumber);
    }

    /// <inheritdoc />
    public async Task<List<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Persons
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Person person, CancellationToken cancellationToken = default)
    {
        await _context.Persons.AddAsync(person, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
