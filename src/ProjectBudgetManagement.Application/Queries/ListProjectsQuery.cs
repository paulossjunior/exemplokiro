using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Query to list projects with optional filtering and pagination.
/// </summary>
public class ListProjectsQuery
{
    /// <summary>
    /// Gets or sets the optional status filter.
    /// </summary>
    public ProjectStatus? Status { get; set; }

    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; } = 50;
}
