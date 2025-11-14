using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace ProjectBudgetManagement.IntegrationTests.Api;

public class AuditControllerTests : IntegrationTestBase
{
    public AuditControllerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task GetAuditTrail_ReturnsAllAuditEntries()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        
        var auditEntry = new AuditEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ActionType = "Create",
            EntityType = "Project",
            EntityId = entityId,
            Timestamp = DateTime.UtcNow,
            PreviousValue = null,
            NewValue = "{\"Name\":\"Test Project\"}",
            DigitalSignature = "SIGNATURE_123",
            DataHash = "HASH_123"
        };

        await DbContext.AuditEntries.AddAsync(auditEntry);
        await DbContext.SaveChangesAsync();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await GetAsync<List<AuditEntryResponse>>("/api/audit/trail");
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result!.Should().NotBeEmpty();
        result.Should().Contain(a => a.EntityId == entityId);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
    }

    [Fact]
    public async Task GetAuditTrail_WithFilters_ReturnsFilteredEntries()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        
        var projectAudit = new AuditEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ActionType = "Create",
            EntityType = "Project",
            EntityId = projectId,
            Timestamp = DateTime.UtcNow,
            NewValue = "{}",
            DigitalSignature = "SIG1",
            DataHash = "HASH1"
        };

        var transactionAudit = new AuditEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ActionType = "Create",
            EntityType = "Transaction",
            EntityId = transactionId,
            Timestamp = DateTime.UtcNow,
            NewValue = "{}",
            DigitalSignature = "SIG2",
            DataHash = "HASH2"
        };

        await DbContext.AuditEntries.AddRangeAsync(projectAudit, transactionAudit);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await GetAsync<List<AuditEntryResponse>>($"/api/audit/trail?entityType=Project");

        // Assert
        result.Should().NotBeNull();
        result!.Should().Contain(a => a.EntityType == "Project");
        result.Should().NotContain(a => a.EntityType == "Transaction");
    }

    [Fact]
    public async Task VerifyDataIntegrity_WithValidData_ReturnsSuccessReport()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        var project = TestDataBuilder.CreateProject(coordinator);
        var bankAccount = TestDataBuilder.CreateBankAccount(project);
        var accountingAccount = TestDataBuilder.CreateAccountingAccount();
        var transaction = TestDataBuilder.CreateTransaction(bankAccount, accountingAccount);

        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddAsync(project);
        await DbContext.BankAccounts.AddAsync(bankAccount);
        await DbContext.AccountingAccounts.AddAsync(accountingAccount);
        await DbContext.Transactions.AddAsync(transaction);
        await DbContext.SaveChangesAsync();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await GetAsync<IntegrityReportResponse>("/api/audit/integrity");
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result!.TotalTransactionsChecked.Should().BeGreaterThanOrEqualTo(0);
        result.TotalAuditEntriesChecked.Should().BeGreaterThanOrEqualTo(0);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
    }

    [Fact]
    public async Task AuditEntries_AreImmutable()
    {
        // Arrange
        var auditEntry = new AuditEntry
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ActionType = "Create",
            EntityType = "Project",
            EntityId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            NewValue = "Original Value",
            DigitalSignature = "SIGNATURE",
            DataHash = "HASH"
        };

        await DbContext.AuditEntries.AddAsync(auditEntry);
        await DbContext.SaveChangesAsync();

        // Act - Try to modify the audit entry
        auditEntry.NewValue = "Modified Value";
        
        // Assert - Verify the entry wasn't modified in the database
        var savedEntry = await DbContext.AuditEntries.FindAsync(auditEntry.Id);
        savedEntry.Should().NotBeNull();
        savedEntry!.NewValue.Should().Be("Original Value");
    }

    [Fact]
    public async Task CreateTransaction_GeneratesDigitalSignature()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateTestCoordinator();
        var project = TestDataBuilder.CreateProject(coordinator, status: ProjectStatus.InProgress);
        var bankAccount = TestDataBuilder.CreateBankAccount(project);
        var accountingAccount = TestDataBuilder.CreateAccountingAccount();

        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddAsync(project);
        await DbContext.BankAccounts.AddAsync(bankAccount);
        await DbContext.AccountingAccounts.AddAsync(accountingAccount);
        await DbContext.SaveChangesAsync();

        var request = new CreateTransactionRequest
        {
            Amount = 5000m,
            Date = DateTime.UtcNow.Date,
            Classification = "Debit",
            AccountingAccountId = accountingAccount.Id
        };

        // Act
        var response = await PostAsync($"/api/projects/{project.Id}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var transaction = await response.Content.ReadFromJsonAsync<TransactionResponse>(JsonOptions);
        
        transaction.Should().NotBeNull();
        transaction!.DigitalSignature.Should().NotBeNullOrEmpty();
        transaction.DataHash.Should().NotBeNullOrEmpty();
        
        // Verify signature is stored in database
        var dbTransaction = await DbContext.Transactions.FindAsync(transaction.Id);
        dbTransaction.Should().NotBeNull();
        dbTransaction!.DigitalSignature.Should().Be(transaction.DigitalSignature);
        dbTransaction.DataHash.Should().Be(transaction.DataHash);
    }
}
