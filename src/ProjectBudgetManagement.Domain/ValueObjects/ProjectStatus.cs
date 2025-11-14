namespace ProjectBudgetManagement.Domain.ValueObjects;

/// <summary>
/// Represents the current state of a project in its lifecycle.
/// </summary>
public enum ProjectStatus
{
    /// <summary>
    /// Project has been created but not yet started.
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Project has been initiated and is in setup phase.
    /// </summary>
    Initiated = 1,

    /// <summary>
    /// Project is actively in progress.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Project has been completed successfully.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Project has been cancelled and will not continue.
    /// </summary>
    Cancelled = 4
}
