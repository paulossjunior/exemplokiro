using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.Infrastructure.Persistence;

namespace ProjectBudgetManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Project entity.
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly ProjectBudgetDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ProjectRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ProjectRepository(ProjectBudgetDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.Coordinator)
            .Include(p => p.BankAccount)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetAllAsync(
        ProjectStatus? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Projects
            .Include(p => p.Coordinator)
            .AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetCountAsync(ProjectStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Projects.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Update(project);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
