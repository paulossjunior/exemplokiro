namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Request DTO for updating a project.
/// </summary>
public class UpdateProjectRequest
{
    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the project start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the project end date.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the project budget amount.
    /// </summary>
    public decimal BudgetAmount { get; set; }

    /// <summary>
    /// Gets or sets the project coordinator ID.
    /// </summary>
    public Guid CoordinatorId { get; set; }
}

/// <summary>
/// Request DTO for updating project status.
/// </summary>
public class UpdateProjectStatusRequest
{
    /// <summary>
    /// Gets or sets the new status.
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
