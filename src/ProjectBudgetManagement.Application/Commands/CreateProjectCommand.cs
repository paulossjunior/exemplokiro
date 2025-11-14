using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Command to create a new project.
/// </summary>
public class CreateProjectCommand
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
    /// Gets or sets the coordinator ID.
    /// </summary>
    public Guid CoordinatorId { get; set; }

    /// <summary>
    /// Gets or sets the bank account number.
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank name.
    /// </summary>
    public string BankName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch number.
    /// </summary>
    public string BranchNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the account holder name.
    /// </summary>
    public string AccountHolderName { get; set; } = string.Empty;
}
