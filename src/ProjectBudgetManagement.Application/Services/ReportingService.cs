using System.Text;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.Services;

namespace ProjectBudgetManagement.Application.Services;

/// <summary>
/// Service for generating accountability reports with audit trails and integrity verification.
/// </summary>
public class ReportingService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IntegrityVerificationService _integrityVerificationService;
    private readonly BalanceCalculationService _balanceCalculationService;

    /// <summary>
    /// Initializes a new instance of the ReportingService class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="transactionRepository">The transaction repository.</param>
    /// <param name="auditRepository">The audit repository.</param>
    /// <param name="integrityVerificationService">The integrity verification service.</param>
    /// <param name="balanceCalculationService">The balance calculation service.</param>
    public ReportingService(
        IProjectRepository projectRepository,
        ITransactionRepository transactionRepository,
        IAuditRepository auditRepository,
        IntegrityVerificationService integrityVerificationService,
        BalanceCalculationService balanceCalculationService)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        _integrityVerificationService = integrityVerificationService ?? throw new ArgumentNullException(nameof(integrityVerificationService));
        _balanceCalculationService = balanceCalculationService ?? throw new ArgumentNullException(nameof(balanceCalculationService));
    }

    /// <summary>
    /// Generates a comprehensive accountability report for a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The accountability report.</returns>
    /// <exception cref="InvalidOperationException">Thrown when project is not found.</exception>
    public async Task<AccountabilityReport> GenerateAccountabilityReportAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        // Get project details
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project with ID {projectId} not found.");
        }

        if (project.BankAccount == null)
        {
            throw new InvalidOperationException($"Project {projectId} does not have an associated bank account.");
        }

        // Get all transactions for the project's bank account
        var transactions = await _transactionRepository.GetByBankAccountAsync(
            project.BankAccount.Id,
            cancellationToken: cancellationToken);

        // Get audit trail for the project
        var auditEntries = await _auditRepository.GetAuditTrailAsync(
            entityId: projectId,
            entityType: nameof(Project),
            cancellationToken: cancellationToken);

        // Get audit entries for all transactions
        var transactionAuditEntries = new List<AuditEntry>();
        foreach (var transaction in transactions)
        {
            var txAudit = await _auditRepository.GetAuditTrailAsync(
                entityId: transaction.Id,
                entityType: nameof(Transaction),
                cancellationToken: cancellationToken);
            transactionAuditEntries.AddRange(txAudit);
        }

        // Combine all audit entries
        var allAuditEntries = auditEntries.Concat(transactionAuditEntries).ToList();

        // Calculate current balance
        var balance = _balanceCalculationService.CalculateBalance(transactions);

        // Verify data integrity
        var integrityReport = _integrityVerificationService.GenerateIntegrityReport(
            transactions,
            allAuditEntries);

        // Generate report
        var report = new AccountabilityReport
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectDescription = project.Description,
            ProjectCoordinator = project.Coordinator?.Name ?? "Unknown",
            BudgetAmount = project.BudgetAmount,
            CurrentBalance = balance,
            BankAccountNumber = project.BankAccount.AccountNumber,
            BankName = project.BankAccount.BankName,
            BranchNumber = project.BankAccount.BranchNumber,
            AccountHolderName = project.BankAccount.AccountHolderName,
            Transactions = transactions.OrderBy(t => t.Date).ToList(),
            AuditEntries = allAuditEntries.OrderBy(a => a.Timestamp).ToList(),
            IntegrityReport = integrityReport,
            GeneratedAt = DateTime.UtcNow,
            ReportIdentifier = GenerateReportIdentifier(projectId)
        };

        return report;
    }

    /// <summary>
    /// Exports an accountability report to PDF format.
    /// </summary>
    /// <param name="report">The accountability report to export.</param>
    /// <returns>The PDF content as byte array.</returns>
    public byte[] ExportToPdf(AccountabilityReport report)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }

        // Generate PDF content
        // Note: This is a placeholder implementation
        // In production, use a PDF library like QuestPDF, iTextSharp, or PdfSharp
        var pdfContent = GeneratePdfContent(report);
        return Encoding.UTF8.GetBytes(pdfContent);
    }

    /// <summary>
    /// Generates a unique report identifier.
    /// </summary>
    private string GenerateReportIdentifier(Guid projectId)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var projectPrefix = projectId.ToString("N").Substring(0, 8).ToUpper();
        return $"REPORT-{projectPrefix}-{timestamp}";
    }

    /// <summary>
    /// Generates PDF content for the report.
    /// This is a placeholder implementation that generates a text-based report.
    /// In production, replace with actual PDF generation library.
    /// </summary>
    private string GeneratePdfContent(AccountabilityReport report)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("=".PadRight(80, '='));
        sb.AppendLine("ACCOUNTABILITY REPORT");
        sb.AppendLine("=".PadRight(80, '='));
        sb.AppendLine();
        sb.AppendLine($"Report ID: {report.ReportIdentifier}");
        sb.AppendLine($"Generated: {report.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();
        
        sb.AppendLine("PROJECT INFORMATION");
        sb.AppendLine("-".PadRight(80, '-'));
        sb.AppendLine($"Project: {report.ProjectName}");
        sb.AppendLine($"Description: {report.ProjectDescription ?? "N/A"}");
        sb.AppendLine($"Coordinator: {report.ProjectCoordinator}");
        sb.AppendLine($"Budget: {report.BudgetAmount:C}");
        sb.AppendLine($"Current Balance: {report.CurrentBalance:C}");
        sb.AppendLine();
        
        sb.AppendLine("BANK ACCOUNT INFORMATION");
        sb.AppendLine("-".PadRight(80, '-'));
        sb.AppendLine($"Bank: {report.BankName}");
        sb.AppendLine($"Branch: {report.BranchNumber}");
        sb.AppendLine($"Account: {report.BankAccountNumber}");
        sb.AppendLine($"Holder: {report.AccountHolderName}");
        sb.AppendLine();
        
        sb.AppendLine("TRANSACTIONS");
        sb.AppendLine("-".PadRight(80, '-'));
        sb.AppendLine($"Total Transactions: {report.Transactions.Count}");
        sb.AppendLine();
        
        foreach (var transaction in report.Transactions)
        {
            var type = transaction.Classification == Domain.ValueObjects.TransactionClassification.Credit ? "CREDIT" : "DEBIT";
            sb.AppendLine($"Date: {transaction.Date:yyyy-MM-dd} | {type} | Amount: {transaction.Amount:C}");
            sb.AppendLine($"  Signature: {transaction.DigitalSignature.Substring(0, Math.Min(40, transaction.DigitalSignature.Length))}...");
            sb.AppendLine();
        }
        
        sb.AppendLine("AUDIT TRAIL");
        sb.AppendLine("-".PadRight(80, '-'));
        sb.AppendLine($"Total Audit Entries: {report.AuditEntries.Count}");
        sb.AppendLine();
        
        foreach (var audit in report.AuditEntries.Take(10)) // Show first 10 for brevity
        {
            sb.AppendLine($"{audit.Timestamp:yyyy-MM-dd HH:mm:ss} | {audit.ActionType} | {audit.EntityType}");
            sb.AppendLine($"  User: {audit.UserId}");
            sb.AppendLine($"  Signature: {audit.DigitalSignature.Substring(0, Math.Min(40, audit.DigitalSignature.Length))}...");
            sb.AppendLine();
        }
        
        if (report.AuditEntries.Count > 10)
        {
            sb.AppendLine($"... and {report.AuditEntries.Count - 10} more audit entries");
            sb.AppendLine();
        }
        
        sb.AppendLine("DATA INTEGRITY VERIFICATION");
        sb.AppendLine("-".PadRight(80, '-'));
        sb.AppendLine($"Verification Time: {report.IntegrityReport.VerificationTimestamp:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Transactions Checked: {report.IntegrityReport.TotalTransactionsChecked}");
        sb.AppendLine($"Audit Entries Checked: {report.IntegrityReport.TotalAuditEntriesChecked}");
        sb.AppendLine($"Integrity Status: {(report.IntegrityReport.IsIntegrityValid ? "VALID" : "COMPROMISED")}");
        
        if (!report.IntegrityReport.IsIntegrityValid)
        {
            sb.AppendLine();
            sb.AppendLine("WARNING: Data integrity issues detected!");
            sb.AppendLine($"Tampered Transactions: {report.IntegrityReport.TamperedTransactionIds.Count}");
            sb.AppendLine($"Tampered Audit Entries: {report.IntegrityReport.TamperedAuditEntryIds.Count}");
        }
        
        sb.AppendLine();
        sb.AppendLine("=".PadRight(80, '='));
        sb.AppendLine("END OF REPORT");
        sb.AppendLine("=".PadRight(80, '='));
        
        return sb.ToString();
    }
}

/// <summary>
/// Represents a comprehensive accountability report for a project.
/// </summary>
public class AccountabilityReport
{
    /// <summary>
    /// Gets or sets the unique report identifier.
    /// </summary>
    public Guid Id { get; set; }

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
    /// Gets or sets the project budget amount.
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
    public List<Transaction> Transactions { get; set; } = new();

    /// <summary>
    /// Gets or sets the audit trail entries.
    /// </summary>
    public List<AuditEntry> AuditEntries { get; set; } = new();

    /// <summary>
    /// Gets or sets the integrity verification report.
    /// </summary>
    public IntegrityReport IntegrityReport { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp when the report was generated.
    /// </summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Gets or sets the unique report identifier string.
    /// </summary>
    public string ReportIdentifier { get; set; } = string.Empty;
}
