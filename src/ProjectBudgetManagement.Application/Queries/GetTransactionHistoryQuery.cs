using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Query to get transaction history for a project with filtering capabilities.
/// </summary>
public class GetTransactionHistoryQuery
{
    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the optional start date filter.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the optional end date filter.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the optional classification filter.
    /// </summary>
    public TransactionClassification? Classification { get; set; }

    /// <summary>
    /// Gets or sets the optional accounting account ID filter.
    /// </summary>
    public Guid? AccountingAccountId { get; set; }
}
