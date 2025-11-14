using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Ports;

/// <summary>
/// Repository interface for AuditEntry entity operations.
/// </summary>
public interface IAuditRepository
{
    /// <summary>
    /// Gets audit entries with optional filtering.
    /// </summary>
    /// <param name="entityId">Optional entity ID filter.</param>
    /// <param name="entityType">Optional entity type filter.</param>
    /// <param name="userId">Optional user ID filter.</param>
    /// <param name="startDate">Optional start date filter.</param>
    /// <param name="endDate">Optional end date filter.</param>
    /// <param name="skip">Number of records to skip for pagination.</param>
    /// <param name="take">Number of records to take for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of audit entries.</returns>
    Task<List<AuditEntry>> GetAuditTrailAsync(
        Guid? entityId = null,
        string? entityType = null,
        Guid? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new audit entry (append-only).
    /// </summary>
    /// <param name="auditEntry">The audit entry to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(AuditEntry auditEntry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
