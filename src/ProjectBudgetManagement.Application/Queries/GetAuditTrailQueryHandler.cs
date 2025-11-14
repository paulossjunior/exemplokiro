using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Handler for getting audit trail with filtering.
/// </summary>
public class GetAuditTrailQueryHandler
{
    private readonly IAuditRepository _auditRepository;

    /// <summary>
    /// Initializes a new instance of the GetAuditTrailQueryHandler class.
    /// </summary>
    /// <param name="auditRepository">The audit repository.</param>
    public GetAuditTrailQueryHandler(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
    }

    /// <summary>
    /// Handles the get audit trail query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of audit entries.</returns>
    public async Task<List<AuditEntry>> HandleAsync(
        GetAuditTrailQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        return await _auditRepository.GetAuditTrailAsync(
            entityId: query.EntityId,
            entityType: query.EntityType,
            userId: query.UserId,
            startDate: query.StartDate,
            endDate: query.EndDate,
            skip: query.Skip,
            take: query.Take,
            cancellationToken: cancellationToken);
    }
}
