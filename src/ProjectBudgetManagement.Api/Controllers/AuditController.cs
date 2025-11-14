using Microsoft.AspNetCore.Mvc;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Application.Queries;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectBudgetManagement.Api.Controllers;

/// <summary>
/// Provides access to audit trails and data integrity verification.
/// </summary>
/// <remarks>
/// The audit system maintains an immutable record of all system operations with digital signatures.
/// Integrity verification uses SHA-256 hashing to detect any data tampering.
/// 
/// **Security**: Audit entries cannot be modified or deleted by any user.
/// 
/// **Performance**: All endpoints respond in &lt;100ms
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuditController : ControllerBase
{
    private readonly GetAuditTrailQueryHandler _getAuditTrailHandler;
    private readonly VerifyDataIntegrityQueryHandler _verifyIntegrityHandler;

    /// <summary>
    /// Initializes a new instance of the AuditController class.
    /// </summary>
    public AuditController(
        GetAuditTrailQueryHandler getAuditTrailHandler,
        VerifyDataIntegrityQueryHandler verifyIntegrityHandler)
    {
        _getAuditTrailHandler = getAuditTrailHandler;
        _verifyIntegrityHandler = verifyIntegrityHandler;
    }

    /// <summary>
    /// Retrieves audit trail with optional filtering and pagination
    /// </summary>
    /// <param name="entityId">Optional filter by entity ID</param>
    /// <param name="entityType">Optional filter by entity type (Project, Transaction, etc.)</param>
    /// <param name="userId">Optional filter by user who performed the action</param>
    /// <param name="startDate">Optional start date filter (inclusive)</param>
    /// <param name="endDate">Optional end date filter (inclusive)</param>
    /// <param name="skip">Number of records to skip for pagination (default: 0)</param>
    /// <param name="take">Number of records to return (default: 100, max: 1000)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Chronological list of audit entries</returns>
    /// <remarks>
    /// Returns the audit trail showing all system operations with user identification and timestamps.
    /// All audit entries are immutable and include digital signatures for non-repudiation.
    /// 
    /// **Filtering Options**:
    /// - Entity: Filter by specific entity ID and/or type
    /// - User: Filter by user who performed actions
    /// - Date range: Filter by timestamp
    /// 
    /// **Entity Types**:
    /// - Project
    /// - Transaction
    /// - AccountingAccount
    /// - BankAccount
    /// 
    /// **Action Types**:
    /// - Created
    /// - Updated
    /// - StatusChanged
    /// - Deleted
    /// 
    /// **Performance**: Responds in &lt;100ms
    /// 
    /// Sample request:
    /// 
    ///     GET /api/audit/trail?entityType=Transaction&amp;startDate=2025-01-01&amp;take=50
    /// 
    /// </remarks>
    /// <response code="200">Returns the list of audit entries. Empty array if no entries match criteria.</response>
    [HttpGet("trail")]
    [SwaggerOperation(
        Summary = "Get audit trail",
        Description = "Retrieves chronological audit trail with optional filtering",
        OperationId = "GetAuditTrail",
        Tags = new[] { "Audit" }
    )]
    [ProducesResponseType(typeof(List<AuditEntryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuditTrail(
        [FromQuery] Guid? entityId,
        [FromQuery] string? entityType,
        [FromQuery] Guid? userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuditTrailQuery
        {
            EntityId = entityId,
            EntityType = entityType,
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            Skip = skip,
            Take = take
        };

        var auditEntries = await _getAuditTrailHandler.HandleAsync(query, cancellationToken);
        var responses = auditEntries.Select(MapToResponse).ToList();

        return Ok(responses);
    }

    /// <summary>
    /// Verifies data integrity for a project using cryptographic hashing
    /// </summary>
    /// <param name="projectId">The project unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Integrity verification report showing any tampering detected</returns>
    /// <remarks>
    /// Performs comprehensive integrity verification on all project data using SHA-256 hashing.
    /// Detects any unauthorized modifications to transactions or audit entries.
    /// 
    /// **Verification Process**:
    /// 1. Recalculates SHA-256 hash for each transaction
    /// 2. Compares calculated hash with stored hash
    /// 3. Recalculates SHA-256 hash for each audit entry
    /// 4. Compares calculated hash with stored hash
    /// 5. Reports any mismatches as tampering
    /// 
    /// **Security Alert**: If tampering is detected, a security alert is logged and 
    /// system administrators are notified.
    /// 
    /// **Performance**: Responds in &lt;100ms
    /// 
    /// Sample request:
    /// 
    ///     GET /api/audit/integrity?projectId=3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// 
    /// </remarks>
    /// <response code="200">Returns integrity report. IsIntegrityValid=true if no tampering detected.</response>
    /// <response code="404">Project not found.</response>
    /// <response code="500">Integrity verification failed due to system error.</response>
    [HttpGet("integrity")]
    [SwaggerOperation(
        Summary = "Verify data integrity",
        Description = "Performs cryptographic integrity verification on project data",
        OperationId = "VerifyIntegrity",
        Tags = new[] { "Audit" }
    )]
    [ProducesResponseType(typeof(IntegrityReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyIntegrity(
        [FromQuery] Guid projectId,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new VerifyDataIntegrityQuery { ProjectId = projectId };
            var report = await _verifyIntegrityHandler.HandleAsync(query, cancellationToken);
            var response = MapToIntegrityResponse(report);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    private static AuditEntryResponse MapToResponse(AuditEntry entry)
    {
        return new AuditEntryResponse
        {
            Id = entry.Id,
            UserId = entry.UserId,
            ActionType = entry.ActionType,
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            Timestamp = entry.Timestamp,
            PreviousValue = entry.PreviousValue,
            NewValue = entry.NewValue,
            DigitalSignature = entry.DigitalSignature,
            DataHash = entry.DataHash
        };
    }

    private static IntegrityReportResponse MapToIntegrityResponse(IntegrityReport report)
    {
        return new IntegrityReportResponse
        {
            VerificationTimestamp = report.VerificationTimestamp,
            TotalTransactionsChecked = report.TotalTransactionsChecked,
            TamperedTransactionIds = report.TamperedTransactionIds,
            TotalAuditEntriesChecked = report.TotalAuditEntriesChecked,
            TamperedAuditEntryIds = report.TamperedAuditEntryIds,
            IsIntegrityValid = report.IsIntegrityValid
        };
    }
}
