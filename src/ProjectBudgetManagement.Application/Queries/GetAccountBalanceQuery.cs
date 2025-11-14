namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Query to get the current account balance for a project.
/// </summary>
public class GetAccountBalanceQuery
{
    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public Guid ProjectId { get; set; }
}
