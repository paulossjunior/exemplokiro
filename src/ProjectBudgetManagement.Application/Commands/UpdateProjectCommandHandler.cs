using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Handler for UpdateProjectCommand.
/// </summary>
public class UpdateProjectCommandHandler
{
    private readonly IProjectRepository _projectRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    public UpdateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Handles the UpdateProjectCommand.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated project.</returns>
    /// <exception cref="InvalidOperationException">Thrown when validation fails or project not found.</exception>
    public async Task<Project> HandleAsync(UpdateProjectCommand command, CancellationToken cancellationToken = default)
    {
        // Get existing project
        var project = await _projectRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project with ID {command.Id} not found.");
        }

        // Update project properties
        project.Name = command.Name;
        project.Description = command.Description;
        project.StartDate = command.StartDate;
        project.EndDate = command.EndDate;
        project.BudgetAmount = command.BudgetAmount;
        project.UpdatedAt = DateTime.UtcNow;

        // Validate updated project
        project.Validate();

        // Update project in repository
        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return project;
    }
}
