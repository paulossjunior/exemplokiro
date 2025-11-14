using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Infrastructure.Persistence;

namespace ProjectBudgetManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for AuditEntry entity.
/// </summary>
public class AuditRepository : IAuditRepository
{
    private readonly ProjectBudgetDbContext _context;

    /// <summary>
    /// Initializes a new instance of the AuditRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AuditRepository(ProjectBudgetDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<List<AuditEntry>> GetAuditTrailAsync(
        Guid? entityId = null,
        string? entityType = null,
        Guid? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditEntries.AsNoTracking();

        if (entityId.HasValue)
        {
            query = query.Where(a => a.EntityId == entityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(a => a.EntityType == entityType);
        }

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= endDate.Value);
        }

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(AuditEntry auditEntry, CancellationToken cancellationToken = default)
    {
        await _context.AuditEntries.AddAsync(auditEntry, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
