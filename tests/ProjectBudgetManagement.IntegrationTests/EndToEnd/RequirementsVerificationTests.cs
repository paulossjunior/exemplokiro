using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;
using Xunit;

namespace ProjectBudgetManagement.IntegrationTests.EndToEnd;

/// <summary>
/// End-to-end tests verifying all requirements are implemented correctly.
/// Tests complete workflows from project creation through reporting.
/// </summary>
[Collection("Database")]
public class RequirementsVerificationTests : IntegrationTestBase
{
    public RequirementsVerificationTests(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task CompleteProjectLifecycle_WorksEndToEnd()
    {
        // Requirement 1: Create project with complete information
        var coordinator = await CreateTestPersonAsync("Jane Smith", "COORD-001");
        
        var createProjectRequest = new CreateProjectRequest
        {
            Name = "Community Center Renovation",
            Description = "Complete renovation of downtown community center",
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            BudgetAmount = 150000.00m,
            CoordinatorId = coordinator.Id,
            BankAccount = new BankAccountRequest
            {
                AccountNumber = "12345678",
                BankName = "First National Bank",
                BranchNumber = "001",
                AccountHolderName = "Jane Smith"
            }
        };

        var createResponse = await Client.PostAsJsonAsync("/api/projects", createProjectRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var project = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        project.Should().NotBeNull();
        project!.Status.Should().Be("NotStarted"); // Requirement 1.5
        project.BudgetAmount.Should().Be(150000.00m);
        
        // Requirement 2: Project has coordinator assigned
        project.CoordinatorId.Should().Be(coordinator.Id);
        
        // Requirement 3: Project has dedicated bank account
        project.BankAccount.Should().NotBeNull();
        project.BankAccount!.AccountNumber.Should().Be("12345678");
        project.BankAccount.BankName.Should().Be("First National Bank");
        
        // Requirement 7: Update project status
        var updateStatusRequest = new UpdateProjectStatusRequest { Status = "InProgress" };
        var statusResponse = await Client.PutAsJsonAsync($"/api/projects/{project.Id}/status", updateStatusRequest);
        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var updatedProject = await statusResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        updatedProject!.Status.Should().Be("InProgress");
    }

    [Fact]
    public async Task TransactionCreationWithAuditTrail_WorksEndToEnd()
    {
        // Setup: Create project with coordinator and accounting account
        var coordinator = await CreateTestPersonAsync("John Doe", "COORD-002");
        var project = await CreateTestProjectAsync(coordinator.Id, "Test Project", 100000.00m);
        var accountingAccount = await CreateTestAccountingAccountAsync("Office Supplies", "4010.01.0001");
        
        // Requirement 4: Create transaction
        var createTransactionRequest = new CreateTransactionRequest
        {
            Amount = 5000.00m,
            Date = DateTime.UtcNow.Date,
            Classification = "Debit",
            AccountingAccountId = accountingAccount.Id
        };

        var response = await Client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/transactions",
            createTransactionRequest);
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var transaction = await response.Content.ReadFromJsonAsync<TransactionResponse>();
        transaction.Should().NotBeNull();
        transaction!.Amount.Should().Be(5000.00m);
        transaction.Classification.Should().Be("Debit");
        
        // Requirement 10: Transaction has digital signature
        transaction.DigitalSignature.Should().NotBeNullOrEmpty();
        transaction.DataHash.Should().NotBeNullOrEmpty();
        
        // Requirement 9: Audit trail entry created
        var auditResponse = await Client.GetAsync($"/api/audit/trail?entityId={transaction.Id}");
        auditResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var auditEntries = await auditResponse.Content.ReadFromJsonAsync<List<AuditEntryResponse>>();
        auditEntries.Should().NotBeNull();
        auditEntries!.Should().ContainSingle(e => e.EntityId == transaction.Id);
        
        var auditEntry = auditEntries.First(e => e.EntityId == transaction.Id);
        auditEntry.ActionType.Should().Be("Create");
        auditEntry.EntityType.Should().Be("Transaction");
        auditEntry.DigitalSignature.Should().NotBeNullOrEmpty();
        auditEntry.DataHash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AccountBalanceCalculation_WorksCorrectly()
    {
        // Setup
        var coordinator = await CreateTestPersonAsync("Alice Johnson", "COORD-003");
        var project = await CreateTestProjectAsync(coordinator.Id, "Balance Test Project", 50000.00m);
        var accountingAccount = await CreateTestAccountingAccountAsync("Revenue", "3000.01.0001");
        
        // Requirement 6: Calculate balance from transactions
        // Create credit transaction (money in)
        await CreateTestTransactionAsync(project.Id, 10000.00m, TransactionClassification.Credit, accountingAccount.Id);
        
        // Create debit transaction (money out)
        await CreateTestTransactionAsync(project.Id, 3000.00m, TransactionClassification.Debit, accountingAccount.Id);
        
        // Get balance
        var balanceResponse = await Client.GetAsync($"/api/projects/{project.Id}/transactions/balance");
        balanceResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var balance = await balanceResponse.Content.ReadFromJsonAsync<AccountBalanceResponse>();
        balance.Should().NotBeNull();
        
        // Balance = Credits - Debits = 10000 - 3000 = 7000
        balance!.Balance.Should().Be(7000.00m);
    }

    [Fact]
    public async Task TransactionHistory_WithFiltering_WorksCorrectly()
    {
        // Setup
        var coordinator = await CreateTestPersonAsync("Bob Wilson", "COORD-004");
        var project = await CreateTestProjectAsync(coordinator.Id, "History Test Project", 75000.00m);
        var account1 = await CreateTestAccountingAccountAsync("Equipment", "5010.01.0001");
        var account2 = await CreateTestAccountingAccountAsync("Services", "5020.01.0001");
        
        // Create multiple transactions
        await CreateTestTransactionAsync(project.Id, 5000.00m, TransactionClassification.Debit, account1.Id, new DateTime(2025, 1, 15));
        await CreateTestTransactionAsync(project.Id, 3000.00m, TransactionClassification.Credit, account2.Id, new DateTime(2025, 2, 10));
        await CreateTestTransactionAsync(project.Id, 2000.00m, TransactionClassification.Debit, account1.Id, new DateTime(2025, 3, 5));
        
        // Requirement 8: Get transaction history with filtering
        var historyResponse = await Client.GetAsync(
            $"/api/projects/{project.Id}/transactions?classification=Debit&startDate=2025-01-01&endDate=2025-03-31");
        
        historyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var transactions = await historyResponse.Content.ReadFromJsonAsync<List<TransactionResponse>>();
        transactions.Should().NotBeNull();
        transactions!.Should().HaveCount(2); // Only debit transactions
        transactions.Should().AllSatisfy(t => t.Classification.Should().Be("Debit"));
    }

    [Fact]
    public async Task ClosedProject_PreventsNewTransactions()
    {
        // Setup
        var coordinator = await CreateTestPersonAsync("Carol Davis", "COORD-005");
        var project = await CreateTestProjectAsync(coordinator.Id, "Closed Project Test", 25000.00m);
        var accountingAccount = await CreateTestAccountingAccountAsync("Expenses", "6000.01.0001");
        
        // Requirement 7.2: Close project
        var updateStatusRequest = new UpdateProjectStatusRequest { Status = "Completed" };
        await Client.PutAsJsonAsync($"/api/projects/{project.Id}/status", updateStatusRequest);
        
        // Try to create transaction on closed project
        var createTransactionRequest = new CreateTransactionRequest
        {
            Amount = 1000.00m,
            Date = DateTime.UtcNow.Date,
            Classification = "Debit",
            AccountingAccountId = accountingAccount.Id
        };

        var response = await Client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/transactions",
            createTransactionRequest);
        
        // Should fail
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AccountabilityReport_GeneratesWithAllData()
    {
        // Setup complete project with transactions
        var coordinator = await CreateTestPersonAsync("David Brown", "COORD-006");
        var project = await CreateTestProjectAsync(coordinator.Id, "Report Test Project", 100000.00m);
        var accountingAccount = await CreateTestAccountingAccountAsync("Materials", "7000.01.0001");
        
        // Create some transactions
        await CreateTestTransactionAsync(project.Id, 15000.00m, TransactionClassification.Debit, accountingAccount.Id);
        await CreateTestTransactionAsync(project.Id, 5000.00m, TransactionClassification.Credit, accountingAccount.Id);
        
        // Requirement 11: Generate accountability report
        var reportResponse = await Client.PostAsync($"/api/reports/accountability/{project.Id}", null);
        reportResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var report = await reportResponse.Content.ReadFromJsonAsync<AccountabilityReportResponse>();
        report.Should().NotBeNull();
        
        // Requirement 11.1: Report contains project details, budget, transactions, and balance
        report!.ProjectName.Should().Be("Report Test Project");
        report.BudgetAmount.Should().Be(100000.00m);
        report.Transactions.Should().HaveCount(2);
        report.CurrentBalance.Should().Be(-10000.00m); // 5000 credit - 15000 debit
        
        // Requirement 11.2: Report includes audit trail
        report.AuditEntries.Should().NotBeEmpty();
        
        // Requirement 11.3: Report includes digital signatures
        report.Transactions.Should().AllSatisfy(t => 
            t.DigitalSignature.Should().NotBeNullOrEmpty());
        
        // Requirement 11.5: Report has unique identifier and timestamp
        report.ReportIdentifier.Should().NotBeNullOrEmpty();
        report.GeneratedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }
}
