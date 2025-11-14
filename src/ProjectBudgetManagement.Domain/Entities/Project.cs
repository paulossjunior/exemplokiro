using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Domain.Entities;

/// <summary>
/// Represents a project with budget allocation and financial tracking.
/// </summary>
public class Project
{
    /// <summary>
    /// Gets or sets the unique identifier for the project.
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
    /// Gets or sets the project status.
    /// </summary>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the project budget amount.
    /// </summary>
    public decimal BudgetAmount { get; set; }

    /// <summary>
    /// Gets or sets the project coordinator ID.
    /// </summary>
    public Guid CoordinatorId { get; set; }

    /// <summary>
    /// Gets or sets the project coordinator.
    /// </summary>
    public virtual Person? Coordinator { get; set; }

    /// <summary>
    /// Gets or sets the bank account associated with this project.
    /// </summary>
    public virtual BankAccount? BankAccount { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the project was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the project was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Validates the project entity.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Project name is required.");
        }

        if (StartDate > EndDate)
        {
            throw new InvalidOperationException("Project start date must be before or equal to end date.");
        }

        if (BudgetAmount <= 0)
        {
            throw new InvalidOperationException("Project budget amount must be greater than zero.");
        }
    }

    /// <summary>
    /// Determines if transactions can be created for this project.
    /// </summary>
    /// <returns>True if transactions are allowed, false otherwise.</returns>
    public bool CanCreateTransactions()
    {
        return Status != ProjectStatus.Completed && Status != ProjectStatus.Cancelled;
    }

    /// <summary>
    /// Updates the project status.
    /// </summary>
    /// <param name="newStatus">The new status to set.</param>
    /// <exception cref="InvalidOperationException">Thrown when status transition is invalid.</exception>
    public void UpdateStatus(ProjectStatus newStatus)
    {
        // Allow any status transition for now
        // More complex validation can be added based on business rules
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}
