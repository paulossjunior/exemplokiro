namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Request DTO for creating a new project.
/// </summary>
public class CreateProjectRequest
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

    /// <summary>
    /// Gets or sets the bank account information.
    /// </summary>
    public BankAccountRequest? BankAccount { get; set; }
}

/// <summary>
/// Request DTO for bank account information.
/// </summary>
public class BankAccountRequest
{
    /// <summary>
    /// Gets or sets the account number.
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
