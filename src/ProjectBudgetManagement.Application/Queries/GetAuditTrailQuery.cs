namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Query to get audit trail with filtering capabilities.
/// </summary>
public class GetAuditTrailQuery
{
    /// <summary>
    /// Gets or sets the optional entity ID filter.
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Gets or sets the optional entity type filter.
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Gets or sets the optional user ID filter.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the optional start date filter.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the optional end date filter.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the number of records to skip for pagination.
    /// </summary>
    public int Skip { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of records to take for pagination.
    /// </summary>
    public int Take { get; set; } = 100;
}
