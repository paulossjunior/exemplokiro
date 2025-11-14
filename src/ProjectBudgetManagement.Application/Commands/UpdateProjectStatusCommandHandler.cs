using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Handler for UpdateProjectStatusCommand.
/// </summary>
public class UpdateProjectStatusCommandHandler
{
    private readonly IProjectRepository _projectRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectStatusCommandHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    public UpdateProjectStatusCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Handles the UpdateProjectStatusCommand.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated project.</returns>
    /// <exception cref="InvalidOperationException">Thrown when project not found.</exception>
    public async Task<Project> HandleAsync(UpdateProjectStatusCommand command, CancellationToken cancellationToken = default)
    {
        // Get existing project
        var project = await _projectRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project with ID {command.Id} not found.");
        }

        // Update status using domain method (includes status transition validation)
        project.UpdateStatus(command.NewStatus);

        // Update project in repository
        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return project;
    }
}
