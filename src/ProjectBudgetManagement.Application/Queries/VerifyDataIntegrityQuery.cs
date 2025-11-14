namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Query to verify data integrity for a project.
/// </summary>
public class VerifyDataIntegrityQuery
{
    /// <summary>
    /// Gets or sets the project ID to verify.
    /// </summary>
    public Guid ProjectId { get; set; }
}
