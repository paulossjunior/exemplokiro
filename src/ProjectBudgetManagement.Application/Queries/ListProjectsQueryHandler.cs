using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Result of listing projects with pagination information.
/// </summary>
public class ListProjectsResult
{
    /// <summary>
    /// Gets or sets the list of projects.
    /// </summary>
    public List<Project> Projects { get; set; } = new();

    /// <summary>
    /// Gets or sets the total count of projects matching the filter.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// Handler for ListProjectsQuery.
/// </summary>
public class ListProjectsQueryHandler
{
    private readonly IProjectRepository _projectRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListProjectsQueryHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    public ListProjectsQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Handles the ListProjectsQuery.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of projects with pagination information.</returns>
    public async Task<ListProjectsResult> HandleAsync(ListProjectsQuery query, CancellationToken cancellationToken = default)
    {
        // Validate pagination parameters
        if (query.Page < 1)
        {
            query.Page = 1;
        }

        if (query.PageSize < 1)
        {
            query.PageSize = 50;
        }

        if (query.PageSize > 100)
        {
            query.PageSize = 100; // Max page size
        }

        // Calculate skip value
        var skip = (query.Page - 1) * query.PageSize;

        // Get projects and total count
        var projects = await _projectRepository.GetAllAsync(
            query.Status,
            skip,
            query.PageSize,
            cancellationToken);

        var totalCount = await _projectRepository.GetCountAsync(query.Status, cancellationToken);

        return new ListProjectsResult
        {
            Projects = projects,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
}
