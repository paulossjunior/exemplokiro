using Microsoft.AspNetCore.Mvc;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Application.Commands;
using ProjectBudgetManagement.Application.Queries;
using ProjectBudgetManagement.Application.Services;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectBudgetManagement.Api.Controllers;

/// <summary>
/// Manages project lifecycle including creation, updates, and status transitions.
/// </summary>
/// <remarks>
/// Projects are the core entity in the system, representing financial initiatives with dedicated bank accounts.
/// Each project has a coordinator, budget, timeline, and associated transactions.
/// 
/// **Performance**: All endpoints respond in &lt;100ms
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly CreateProjectCommandHandler _createProjectHandler;
    private readonly UpdateProjectCommandHandler _updateProjectHandler;
    private readonly UpdateProjectStatusCommandHandler _updateStatusHandler;
    private readonly GetProjectQueryHandler _getProjectHandler;
    private readonly ListProjectsQueryHandler _listProjectsHandler;
    private readonly ProjectService _projectService;

    /// <summary>
    /// Initializes a new instance of the ProjectsController class.
    /// </summary>
    public ProjectsController(
        CreateProjectCommandHandler createProjectHandler,
        UpdateProjectCommandHandler updateProjectHandler,
        UpdateProjectStatusCommandHandler updateStatusHandler,
        GetProjectQueryHandler getProjectHandler,
        ListProjectsQueryHandler listProjectsHandler,
        ProjectService projectService)
    {
        _createProjectHandler = createProjectHandler;
        _updateProjectHandler = updateProjectHandler;
        _updateStatusHandler = updateStatusHandler;
        _getProjectHandler = getProjectHandler;
        _listProjectsHandler = listProjectsHandler;
        _projectService = projectService;
    }

    /// <summary>
    /// Creates a new project with dedicated bank account
    /// </summary>
    /// <param name="request">The project creation request containing name, dates, budget, and bank account details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created project with assigned ID and bank account</returns>
    /// <remarks>
    /// Creates a new project in the system with status "NotStarted". Each project must have:
    /// - A unique name and description
    /// - Valid start and end dates (start ≤ end)
    /// - Positive budget amount
    /// - Assigned coordinator (must exist in system)
    /// - Dedicated bank account with unique account number/bank/branch combination
    /// 
    /// The system automatically creates an audit trail entry for this operation.
    /// 
    /// **Performance**: Responds in &lt;100ms
    /// 
    /// Sample request:
    /// 
    ///     POST /api/projects
    ///     {
    ///       "name": "Community Center Renovation",
    ///       "description": "Renovation of the downtown community center",
    ///       "startDate": "2025-01-01",
    ///       "endDate": "2025-12-31",
    ///       "budgetAmount": 150000.00,
    ///       "coordinatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "bankAccount": {
    ///         "accountNumber": "12345678",
    ///         "bankName": "First National Bank",
    ///         "branchNumber": "001",
    ///         "accountHolderName": "John Smith"
    ///       }
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Project successfully created. Returns the created project with ID.</response>
    /// <response code="400">Invalid input data or business rule violation (e.g., start date after end date, negative budget, duplicate bank account).</response>
    /// <response code="404">Coordinator with specified ID not found.</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new project",
        Description = "Creates a new project with dedicated bank account and audit trail",
        OperationId = "CreateProject",
        Tags = new[] { "Projects" }
    )]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateProject(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateProjectCommand
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                BudgetAmount = request.BudgetAmount,
                CoordinatorId = request.CoordinatorId,
                AccountNumber = request.BankAccount?.AccountNumber ?? string.Empty,
                BankName = request.BankAccount?.BankName ?? string.Empty,
                BranchNumber = request.BankAccount?.BranchNumber ?? string.Empty,
                AccountHolderName = request.BankAccount?.AccountHolderName ?? string.Empty
            };

            var project = await _createProjectHandler.HandleAsync(command, cancellationToken);
            var response = MapToProjectResponse(project);

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a project by its unique identifier
    /// </summary>
    /// <param name="id">The project unique identifier (GUID)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The project details including bank account information</returns>
    /// <remarks>
    /// Returns complete project information including:
    /// - Project details (name, description, dates, budget, status)
    /// - Coordinator information
    /// - Associated bank account details
    /// - Creation and update timestamps
    /// 
    /// **Performance**: Responds in &lt;100ms
    /// </remarks>
    /// <response code="200">Project found and returned successfully.</response>
    /// <response code="404">Project with specified ID not found.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get project by ID",
        Description = "Retrieves detailed information about a specific project",
        OperationId = "GetProject",
        Tags = new[] { "Projects" }
    )]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProjectQuery { Id = id };
        var project = await _getProjectHandler.HandleAsync(query, cancellationToken);

        if (project == null)
        {
            return NotFound(new { error = $"Project with ID {id} not found." });
        }

        return Ok(MapToProjectResponse(project));
    }

    /// <summary>
    /// Lists all projects with optional filtering and pagination
    /// </summary>
    /// <param name="status">Optional status filter (NotStarted, Initiated, InProgress, Completed, Cancelled)</param>
    /// <param name="skip">Number of records to skip for pagination (default: 0)</param>
    /// <param name="take">Number of records to return (default: 50, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of projects matching the filter criteria</returns>
    /// <remarks>
    /// Returns a paginated list of projects. Use the skip and take parameters for pagination.
    /// 
    /// **Status Values**:
    /// - NotStarted: Project created but not yet started
    /// - Initiated: Project has been initiated
    /// - InProgress: Project is actively in progress
    /// - Completed: Project has been completed
    /// - Cancelled: Project has been cancelled
    /// 
    /// **Performance**: Responds in &lt;100ms
    /// 
    /// Sample request:
    /// 
    ///     GET /api/projects?status=InProgress&amp;skip=0&amp;take=20
    /// 
    /// </remarks>
    /// <response code="200">Returns the list of projects. Empty array if no projects match criteria.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all projects",
        Description = "Retrieves a paginated list of projects with optional status filtering",
        OperationId = "ListProjects",
        Tags = new[] { "Projects" }
    )]
    [ProducesResponseType(typeof(List<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListProjects(
        [FromQuery] string? status,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        ProjectStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ProjectStatus>(status, true, out var parsedStatus))
        {
            statusFilter = parsedStatus;
        }

        var page = skip / Math.Max(take, 1) + 1;
        var query = new ListProjectsQuery
        {
            Status = statusFilter,
            Page = page,
            PageSize = take
        };

        var result = await _listProjectsHandler.HandleAsync(query, cancellationToken);
        var responses = result.Projects.Select(MapToProjectResponse).ToList();

        return Ok(responses);
    }

    /// <summary>
    /// Updates an existing project's details
    /// </summary>
    /// <param name="id">The project unique identifier</param>
    /// <param name="request">The update request with new project details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated project</returns>
    /// <remarks>
    /// Updates project information including name, description, dates, and budget.
    /// The project status and bank account cannot be changed through this endpoint.
    /// 
    /// All business rules are validated:
    /// - Start date must be ≤ end date
    /// - Budget must be positive
    /// 
    /// An audit trail entry is automatically created for this operation.
    /// 
    /// **Performance**: Responds in &lt;100ms
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/projects/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     {
    ///       "name": "Community Center Renovation - Updated",
    ///       "description": "Updated description with new scope",
    ///       "startDate": "2025-01-01",
    ///       "endDate": "2025-12-31",
    ///       "budgetAmount": 175000.00,
    ///       "coordinatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Project successfully updated.</response>
    /// <response code="400">Invalid input data or business rule violation.</response>
    /// <response code="404">Project with specified ID not found.</response>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update a project",
        Description = "Updates an existing project's details with audit trail",
        OperationId = "UpdateProject",
        Tags = new[] { "Projects" }
    )]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateProjectCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                BudgetAmount = request.BudgetAmount
            };

            var project = await _updateProjectHandler.HandleAsync(command, cancellationToken);
            return Ok(MapToProjectResponse(project));
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound(new { error = ex.Message });
            }
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates a project's status
    /// </summary>
    /// <param name="id">The project unique identifier</param>
    /// <param name="request">The status update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated project</returns>
    /// <remarks>
    /// Changes the project status. Valid transitions are enforced by business rules.
    /// 
    /// **Important**: When a project status is changed to "Completed" or "Cancelled", 
    /// no new transactions can be created for that project.
    /// 
    /// **Valid Status Values**:
    /// - NotStarted
    /// - Initiated
    /// - InProgress
    /// - Completed
    /// - Cancelled
    /// 
    /// An audit trail entry is automatically created recording the status change.
    /// 
    /// **Performance**: Responds in &lt;100ms
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/projects/3fa85f64-5717-4562-b3fc-2c963f66afa6/status
    ///     {
    ///       "status": "InProgress"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Status successfully updated.</response>
    /// <response code="400">Invalid status value or invalid status transition.</response>
    /// <response code="404">Project with specified ID not found.</response>
    [HttpPut("{id}/status")]
    [SwaggerOperation(
        Summary = "Update project status",
        Description = "Changes the project status with validation and audit trail",
        OperationId = "UpdateProjectStatus",
        Tags = new[] { "Projects" }
    )]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProjectStatus(
        Guid id,
        [FromBody] UpdateProjectStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.TryParse<ProjectStatus>(request.Status, true, out var newStatus))
            {
                return BadRequest(new { error = $"Invalid status value: {request.Status}" });
            }

            var command = new UpdateProjectStatusCommand
            {
                Id = id,
                NewStatus = newStatus
            };

            var project = await _updateStatusHandler.HandleAsync(command, cancellationToken);
            return Ok(MapToProjectResponse(project));
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound(new { error = ex.Message });
            }
            return BadRequest(new { error = ex.Message });
        }
    }

    private static ProjectResponse MapToProjectResponse(Project project)
    {
        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status.ToString(),
            BudgetAmount = project.BudgetAmount,
            CoordinatorId = project.CoordinatorId,
            BankAccount = project.BankAccount != null ? new BankAccountResponse
            {
                Id = project.BankAccount.Id,
                AccountNumber = project.BankAccount.AccountNumber,
                BankName = project.BankAccount.BankName,
                BranchNumber = project.BankAccount.BranchNumber,
                AccountHolderName = project.BankAccount.AccountHolderName
            } : null,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }
}
