namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Command to update an existing project.
/// </summary>
public class UpdateProjectCommand
{
    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public Guid Id { get; set; }

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
}
