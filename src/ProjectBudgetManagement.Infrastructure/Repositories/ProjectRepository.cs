using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.Infrastructure.Persistence;
using ProjectBudgetManagement.Infrastructure.Services;

namespace ProjectBudgetManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Project entity.
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly ProjectBudgetDbContext _context;
    private readonly CachingService? _cachingService;

    // Compiled query for frequently accessed project by ID with related entities
    private static readonly Func<ProjectBudgetDbContext, Guid, Task<Project?>> GetProjectByIdCompiled =
        EF.CompileAsyncQuery((ProjectBudgetDbContext context, Guid id) =>
            context.Projects
                .Include(p => p.Coordinator)
                .Include(p => p.BankAccount)
                .FirstOrDefault(p => p.Id == id));

    // Compiled query for counting projects by status
    private static readonly Func<ProjectBudgetDbContext, ProjectStatus?, Task<int>> GetProjectCountCompiled =
        EF.CompileAsyncQuery((ProjectBudgetDbContext context, ProjectStatus? status) =>
            status.HasValue
                ? context.Projects.Count(p => p.Status == status.Value)
                : context.Projects.Count());

    /// <summary>
    /// Initializes a new instance of the ProjectRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="cachingService">Optional caching service for performance optimization.</param>
    public ProjectRepository(ProjectBudgetDbContext context, CachingService? cachingService = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cachingService = cachingService;
    }

    /// <inheritdoc />
    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Use caching if available for better performance
        if (_cachingService != null)
        {
            return await _cachingService.GetOrSetProjectAsync(
                id,
                async () => await GetProjectByIdCompiled(_context, id),
                cancellationToken);
        }

        return await GetProjectByIdCompiled(_context, id);
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
        return await GetProjectCountCompiled(_context, status);
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
        
        // Invalidate cache for this project
        _cachingService?.InvalidateProjectCache(project.Id);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> BankAccountExistsAsync(
        string accountNumber,
        string bankName,
        string branchNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.BankAccounts
            .AsNoTracking()
            .AnyAsync(b => 
                b.AccountNumber == accountNumber && 
                b.BankName == bankName && 
                b.BranchNumber == branchNumber,
                cancellationToken);
    }
}
