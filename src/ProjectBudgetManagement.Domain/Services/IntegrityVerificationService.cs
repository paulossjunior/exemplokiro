using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Domain.Services;

/// <summary>
/// Service for verifying data integrity and detecting tampering.
/// </summary>
public class IntegrityVerificationService
{
    private readonly CryptographicService _cryptographicService;

    /// <summary>
    /// Initializes a new instance of the IntegrityVerificationService class.
    /// </summary>
    /// <param name="cryptographicService">The cryptographic service for hash operations.</param>
    public IntegrityVerificationService(CryptographicService cryptographicService)
    {
        _cryptographicService = cryptographicService ?? throw new ArgumentNullException(nameof(cryptographicService));
    }

    /// <summary>
    /// Validates the data hash of a transaction.
    /// </summary>
    /// <param name="transaction">The transaction to validate.</param>
    /// <returns>True if hash is valid, false if tampering detected.</returns>
    public bool ValidateTransactionIntegrity(Transaction transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        var expectedHash = _cryptographicService.ComputeTransactionHash(
            transaction.Amount,
            transaction.Date,
            (int)transaction.Classification,
            transaction.BankAccountId,
            transaction.AccountingAccountId,
            transaction.CreatedBy);

        return string.Equals(transaction.DataHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates the data hash of an audit entry.
    /// </summary>
    /// <param name="auditEntry">The audit entry to validate.</param>
    /// <returns>True if hash is valid, false if tampering detected.</returns>
    public bool ValidateAuditEntryIntegrity(AuditEntry auditEntry)
    {
        if (auditEntry == null)
        {
            throw new ArgumentNullException(nameof(auditEntry));
        }

        var expectedHash = _cryptographicService.ComputeAuditEntryHash(
            auditEntry.UserId,
            auditEntry.ActionType,
            auditEntry.EntityType,
            auditEntry.EntityId,
            auditEntry.Timestamp,
            auditEntry.PreviousValue,
            auditEntry.NewValue);

        return string.Equals(auditEntry.DataHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Detects tampering in a collection of transactions.
    /// </summary>
    /// <param name="transactions">The transactions to check.</param>
    /// <returns>A list of transaction IDs that failed integrity check.</returns>
    public List<Guid> DetectTamperedTransactions(IEnumerable<Transaction> transactions)
    {
        var tamperedIds = new List<Guid>();

        foreach (var transaction in transactions)
        {
            if (!ValidateTransactionIntegrity(transaction))
            {
                tamperedIds.Add(transaction.Id);
            }
        }

        return tamperedIds;
    }

    /// <summary>
    /// Detects tampering in a collection of audit entries.
    /// </summary>
    /// <param name="auditEntries">The audit entries to check.</param>
    /// <returns>A list of audit entry IDs that failed integrity check.</returns>
    public List<Guid> DetectTamperedAuditEntries(IEnumerable<AuditEntry> auditEntries)
    {
        var tamperedIds = new List<Guid>();

        foreach (var auditEntry in auditEntries)
        {
            if (!ValidateAuditEntryIntegrity(auditEntry))
            {
                tamperedIds.Add(auditEntry.Id);
            }
        }

        return tamperedIds;
    }

    /// <summary>
    /// Generates an integrity report for a project's data.
    /// </summary>
    /// <param name="transactions">The transactions to verify.</param>
    /// <param name="auditEntries">The audit entries to verify.</param>
    /// <returns>An integrity report with verification results.</returns>
    public IntegrityReport GenerateIntegrityReport(
        IEnumerable<Transaction> transactions,
        IEnumerable<AuditEntry> auditEntries)
    {
        var tamperedTransactions = DetectTamperedTransactions(transactions);
        var tamperedAuditEntries = DetectTamperedAuditEntries(auditEntries);

        return new IntegrityReport
        {
            VerificationTimestamp = DateTime.UtcNow,
            TotalTransactionsChecked = transactions.Count(),
            TamperedTransactionIds = tamperedTransactions,
            TotalAuditEntriesChecked = auditEntries.Count(),
            TamperedAuditEntryIds = tamperedAuditEntries,
            IsIntegrityValid = !tamperedTransactions.Any() && !tamperedAuditEntries.Any()
        };
    }
}

/// <summary>
/// Represents an integrity verification report.
/// </summary>
public class IntegrityReport
{
    /// <summary>
    /// Gets or sets the timestamp when verification was performed.
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
