namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Command to generate an accountability report for a project.
/// </summary>
public class GenerateAccountabilityReportCommand
{
    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public Guid ProjectId { get; set; }
}
