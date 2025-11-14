using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Queries;

/// <summary>
/// Handler for GetProjectQuery.
/// </summary>
public class GetProjectQueryHandler
{
    private readonly IProjectRepository _projectRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectQueryHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    public GetProjectQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Handles the GetProjectQuery.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project if found, null otherwise.</returns>
    public async Task<Project?> HandleAsync(GetProjectQuery query, CancellationToken cancellationToken = default)
    {
        return await _projectRepository.GetByIdAsync(query.Id, cancellationToken);
    }
}
