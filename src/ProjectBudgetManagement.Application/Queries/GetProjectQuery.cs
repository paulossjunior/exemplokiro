namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Query to get a project by ID.
/// </summary>
public class GetProjectQuery
{
    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public Guid Id { get; set; }
}
