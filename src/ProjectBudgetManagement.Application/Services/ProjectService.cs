using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Application.Services;

/// <summary>
/// Service for managing projects with audit logging.
/// </summary>
public class ProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly AuditService _auditService;

    /// <summary>
    /// Initializes a new instance of the ProjectService class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="auditService">The audit service.</param>
    public ProjectService(
        IProjectRepository projectRepository,
        AuditService auditService)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
    }

    /// <summary>
    /// Creates a new project with audit logging.
    /// </summary>
    /// <param name="project">The project to create.</param>
    /// <param name="userId">The user ID creating the project.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created project.</returns>
    public async Task<Project> CreateProjectAsync(
        Project project,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        // Validate business rules
        project.Validate();

        // Set timestamps
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        // Save project
        await _projectRepository.AddAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        // Log creation in audit trail
        await _auditService.LogCreateAsync(userId, project, cancellationToken);

        return project;
    }

    /// <summary>
    /// Updates an existing project with audit logging.
    /// </summary>
    /// <param name="projectId">The project ID to update.</param>
    /// <param name="updatedProject">The updated project data.</param>
    /// <param name="userId">The user ID performing the update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated project.</returns>
    /// <exception cref="InvalidOperationException">Thrown when project is not found.</exception>
    public async Task<Project> UpdateProjectAsync(
        Guid projectId,
        Project updatedProject,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (updatedProject == null)
        {
            throw new ArgumentNullException(nameof(updatedProject));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        // Get existing project
        var existingProject = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (existingProject == null)
        {
            throw new InvalidOperationException($"Project with ID {projectId} not found.");
        }

        // Create a copy of the previous state for audit logging
        var previousProject = new Project
        {
            Id = existingProject.Id,
            Name = existingProject.Name,
            Description = existingProject.Description,
            StartDate = existingProject.StartDate,
            EndDate = existingProject.EndDate,
            Status = existingProject.Status,
            BudgetAmount = existingProject.BudgetAmount,
            CoordinatorId = existingProject.CoordinatorId,
            CreatedAt = existingProject.CreatedAt,
            UpdatedAt = existingProject.UpdatedAt
        };

        // Update project properties
        existingProject.Name = updatedProject.Name;
        existingProject.Description = updatedProject.Description;
        existingProject.StartDate = updatedProject.StartDate;
        existingProject.EndDate = updatedProject.EndDate;
        existingProject.BudgetAmount = updatedProject.BudgetAmount;
        existingProject.CoordinatorId = updatedProject.CoordinatorId;
        existingProject.UpdatedAt = DateTime.UtcNow;

        // Validate business rules
        existingProject.Validate();

        // Save changes
        await _projectRepository.UpdateAsync(existingProject, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        // Log update in audit trail
        await _auditService.LogUpdateAsync(userId, previousProject, existingProject, cancellationToken);

        return existingProject;
    }

    /// <summary>
    /// Updates project status with audit logging and validation.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="newStatus">The new status to set.</param>
    /// <param name="userId">The user ID performing the status change.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated project.</returns>
    /// <exception cref="InvalidOperationException">Thrown when project is not found.</exception>
    public async Task<Project> UpdateProjectStatusAsync(
        Guid projectId,
        ProjectStatus newStatus,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        // Get existing project
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project with ID {projectId} not found.");
        }

        // Capture previous status for audit logging
        var previousStatus = project.Status;

        // Update status
        project.UpdateStatus(newStatus);

        // Save changes
        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        // Log status change in audit trail
        await _auditService.LogStatusChangeAsync(
            userId,
            nameof(Project),
            projectId,
            previousStatus.ToString(),
            newStatus.ToString(),
            cancellationToken);

        return project;
    }

    /// <summary>
    /// Gets a project by ID.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project if found, null otherwise.</returns>
    public async Task<Project?> GetProjectByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await _projectRepository.GetByIdAsync(projectId, cancellationToken);
    }

    /// <summary>
    /// Gets all projects with optional filtering.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="skip">Number of records to skip for pagination.</param>
    /// <param name="take">Number of records to take for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of projects.</returns>
    public async Task<List<Project>> GetProjectsAsync(
        ProjectStatus? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        return await _projectRepository.GetAllAsync(status, skip, take, cancellationToken);
    }

    /// <summary>
    /// Gets the total count of projects with optional filtering.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Total count of projects.</returns>
    public async Task<int> GetProjectCountAsync(
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        return await _projectRepository.GetCountAsync(status, cancellationToken);
    }
}
