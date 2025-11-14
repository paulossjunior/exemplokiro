namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Response DTO for integrity verification report.
/// </summary>
public class IntegrityReportResponse
{
    /// <summary>
    /// Gets or sets the verification timestamp.
    /// </summary>
    public DateTime VerificationTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the total number of transactions checked.
    /// </summary>
    public int TotalTransactionsChecked { get; set; }

    /// <summary>
    /// Gets or sets the list of tampered transaction IDs.
    /// </summary>
    public List<Guid> TamperedTransactionIds { get; set; } = new();

    /// <summary>
    /// Gets or sets the total number of audit entries checked.
    /// </summary>
    public int TotalAuditEntriesChecked { get; set; }

    /// <summary>
    /// Gets or sets the list of tampered audit entry IDs.
    /// </summary>
    public List<Guid> TamperedAuditEntryIds { get; set; } = new();

    /// <summary>
    /// Gets or sets whether overall integrity is valid.
    /// </summary>
    public bool IsIntegrityValid { get; set; }
}
