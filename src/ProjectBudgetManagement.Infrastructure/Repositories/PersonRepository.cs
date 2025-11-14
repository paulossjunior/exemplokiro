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
        return await _context.Persons
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Person?> GetByIdentificationNumberAsync(string identificationNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Persons
            .FirstOrDefaultAsync(p => p.IdentificationNumber == identificationNumber, cancellationToken);
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
