using System.Text.Json;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.Services;

namespace ProjectBudgetManagement.Application.Services;

/// <summary>
/// Service for creating and managing audit trail entries.
/// </summary>
public class AuditService
{
    private readonly IAuditRepository _auditRepository;
    private readonly CryptographicService _cryptographicService;
    private readonly DigitalSignatureService _digitalSignatureService;

    /// <summary>
    /// Initializes a new instance of the AuditService class.
    /// </summary>
    /// <param name="auditRepository">The audit repository.</param>
    /// <param name="cryptographicService">The cryptographic service.</param>
    /// <param name="digitalSignatureService">The digital signature service.</param>
    public AuditService(
        IAuditRepository auditRepository,
        CryptographicService cryptographicService,
        DigitalSignatureService digitalSignatureService)
    {
        _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        _cryptographicService = cryptographicService ?? throw new ArgumentNullException(nameof(cryptographicService));
        _digitalSignatureService = digitalSignatureService ?? throw new ArgumentNullException(nameof(digitalSignatureService));
    }

    /// <summary>
    /// Creates an audit entry for entity creation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="userId">The user ID performing the action.</param>
    /// <param name="entity">The created entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task LogCreateAsync<T>(Guid userId, T entity, CancellationToken cancellationToken = default)
        where T : class
    {
        var entityId = GetEntityId(entity);
        var entityType = typeof(T).Name;
        var newValue = SerializeEntity(entity);

        await CreateAuditEntryAsync(
            userId,
            "Create",
            entityType,
            entityId,
            previousValue: null,
            newValue: newValue,
            cancellationToken);
    }

    /// <summary>
    /// Creates an audit entry for entity update.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="userId">The user ID performing the action.</param>
    /// <param name="previousEntity">The entity state before update.</param>
    /// <param name="newEntity">The entity state after update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task LogUpdateAsync<T>(Guid userId, T previousEntity, T newEntity, CancellationToken cancellationToken = default)
        where T : class
    {
        var entityId = GetEntityId(newEntity);
        var entityType = typeof(T).Name;
        var previousValue = SerializeEntity(previousEntity);
        var newValue = SerializeEntity(newEntity);

        await CreateAuditEntryAsync(
            userId,
            "Update",
            entityType,
            entityId,
            previousValue,
            newValue,
            cancellationToken);
    }

    /// <summary>
    /// Creates an audit entry for entity deletion.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="userId">The user ID performing the action.</param>
    /// <param name="entity">The deleted entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task LogDeleteAsync<T>(Guid userId, T entity, CancellationToken cancellationToken = default)
        where T : class
    {
        var entityId = GetEntityId(entity);
        var entityType = typeof(T).Name;
        var previousValue = SerializeEntity(entity);

        await CreateAuditEntryAsync(
            userId,
            "Delete",
            entityType,
            entityId,
            previousValue,
            newValue: null,
            cancellationToken);
    }

    /// <summary>
    /// Creates an audit entry for status change.
    /// </summary>
    /// <param name="userId">The user ID performing the action.</param>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The entity ID.</param>
    /// <param name="previousStatus">The previous status.</param>
    /// <param name="newStatus">The new status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task LogStatusChangeAsync(
        Guid userId,
        string entityType,
        Guid entityId,
        string previousStatus,
        string newStatus,
        CancellationToken cancellationToken = default)
    {
        var previousValue = JsonSerializer.Serialize(new { Status = previousStatus });
        var newValue = JsonSerializer.Serialize(new { Status = newStatus });

        await CreateAuditEntryAsync(
            userId,
            "StatusChange",
            entityType,
            entityId,
            previousValue,
            newValue,
            cancellationToken);
    }

    /// <summary>
    /// Creates an audit entry for transaction creation with digital signature.
    /// </summary>
    /// <param name="userId">The user ID performing the action.</param>
    /// <param name="transaction">The created transaction.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task LogTransactionAsync(Guid userId, Transaction transaction, CancellationToken cancellationToken = default)
    {
        var entityType = nameof(Transaction);
        var newValue = SerializeEntity(transaction);

        await CreateAuditEntryAsync(
            userId,
            "CreateTransaction",
            entityType,
            transaction.Id,
            previousValue: null,
            newValue: newValue,
            cancellationToken);
    }

    /// <summary>
    /// Creates an immutable audit entry with digital signature and hash.
    /// </summary>
    private async Task CreateAuditEntryAsync(
        Guid userId,
        string actionType,
        string entityType,
        Guid entityId,
        string? previousValue,
        string? newValue,
        CancellationToken cancellationToken)
    {
        var timestamp = DateTime.UtcNow;

        // Generate digital signature for non-repudiation
        var signature = _digitalSignatureService.GenerateAuditSignature(
            actionType,
            entityType,
            entityId,
            timestamp,
            userId);

        // Compute cryptographic hash for integrity verification
        var dataHash = _cryptographicService.ComputeAuditEntryHash(
            userId,
            actionType,
            entityType,
            entityId,
            timestamp,
            previousValue,
            newValue);

        var auditEntry = new AuditEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ActionType = actionType,
            EntityType = entityType,
            EntityId = entityId,
            Timestamp = timestamp,
            PreviousValue = previousValue,
            NewValue = newValue,
            DigitalSignature = signature,
            DataHash = dataHash
        };

        // Validate before saving to ensure immutability requirements
        auditEntry.Validate();

        await _auditRepository.AddAsync(auditEntry, cancellationToken);
        await _auditRepository.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Extracts the entity ID from an entity object.
    /// </summary>
    private Guid GetEntityId(object entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        if (idProperty == null)
        {
            throw new InvalidOperationException($"Entity type {entity.GetType().Name} does not have an Id property.");
        }

        var id = idProperty.GetValue(entity);
        if (id is Guid guidId)
        {
            return guidId;
        }

        throw new InvalidOperationException($"Entity Id is not of type Guid.");
    }

    /// <summary>
    /// Serializes an entity to JSON for audit trail storage.
    /// </summary>
    private string SerializeEntity(object entity)
    {
        return JsonSerializer.Serialize(entity, new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }
}
