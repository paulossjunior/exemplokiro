using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Command to update a project's status.
/// </summary>
public class UpdateProjectStatusCommand
{
    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the new status.
    /// </summary>
    public ProjectStatus NewStatus { get; set; }
}
