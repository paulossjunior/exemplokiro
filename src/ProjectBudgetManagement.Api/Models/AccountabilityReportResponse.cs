namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Response DTO for accountability report.
/// </summary>
public class AccountabilityReportResponse
{
    /// <summary>
    /// Gets or sets the report ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the report identifier.
    /// </summary>
    public string ReportIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project description.
    /// </summary>
    public string? ProjectDescription { get; set; }

    /// <summary>
    /// Gets or sets the project coordinator name.
    /// </summary>
    public string ProjectCoordinator { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the budget amount.
    /// </summary>
    public decimal BudgetAmount { get; set; }

    /// <summary>
    /// Gets or sets the current balance.
    /// </summary>
    public decimal CurrentBalance { get; set; }

    /// <summary>
    /// Gets or sets the bank account number.
    /// </summary>
    public string BankAccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank name.
    /// </summary>
    public string BankName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch number.
    /// </summary>
    public string BranchNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the account holder name.
    /// </summary>
    public string AccountHolderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of transactions.
    /// </summary>
    public List<TransactionResponse> Transactions { get; set; } = new();

    /// <summary>
    /// Gets or sets the audit trail entries.
    /// </summary>
    public List<AuditEntryResponse> AuditEntries { get; set; } = new();

    /// <summary>
    /// Gets or sets the integrity report.
    /// </summary>
    public IntegrityReportResponse IntegrityReport { get; set; } = new();

    /// <summary>
    /// Gets or sets the generation timestamp.
    /// </summary>
    public DateTime GeneratedAt { get; set; }
}
