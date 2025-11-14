namespace ProjectBudgetManagement.Domain.Entities;

/// <summary>
/// Represents an immutable audit trail entry for system actions.
/// </summary>
public class AuditEntry
{
    /// <summary>
    /// Gets or sets the unique identifier for the audit entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID who performed the action.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the type of action performed (e.g., "Create", "Update", "Delete").
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of entity affected (e.g., "Project", "Transaction").
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the entity affected.
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the action occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the previous value before the action (JSON serialized).
    /// </summary>
    public string? PreviousValue { get; set; }

    /// <summary>
    /// Gets or sets the new value after the action (JSON serialized).
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the digital signature for non-repudiation.
    /// </summary>
    public string DigitalSignature { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cryptographic hash for data integrity verification.
    /// </summary>
    public string DataHash { get; set; } = string.Empty;

    /// <summary>
    /// Validates the audit entry entity.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (UserId == Guid.Empty)
        {
            throw new InvalidOperationException("Audit entry must have a valid user ID.");
        }

        if (string.IsNullOrWhiteSpace(ActionType))
        {
            throw new InvalidOperationException("Audit entry must have an action type.");
        }

        if (string.IsNullOrWhiteSpace(EntityType))
        {
            throw new InvalidOperationException("Audit entry must have an entity type.");
        }

        if (EntityId == Guid.Empty)
        {
            throw new InvalidOperationException("Audit entry must have a valid entity ID.");
        }

        if (string.IsNullOrWhiteSpace(DigitalSignature))
        {
            throw new InvalidOperationException("Audit entry must have a digital signature.");
        }

        if (string.IsNullOrWhiteSpace(DataHash))
        {
            throw new InvalidOperationException("Audit entry must have a data hash.");
        }
    }
}
