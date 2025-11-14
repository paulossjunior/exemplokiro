namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Response DTO for audit entry information.
/// </summary>
public class AuditEntryResponse
{
    /// <summary>
    /// Gets or sets the audit entry ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID who performed the action.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the action type.
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entity type.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entity ID.
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the previous value.
    /// </summary>
    public string? PreviousValue { get; set; }

    /// <summary>
    /// Gets or sets the new value.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the digital signature.
    /// </summary>
    public string DigitalSignature { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data hash.
    /// </summary>
    public string DataHash { get; set; } = string.Empty;
}
