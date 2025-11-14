using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace ProjectBudgetManagement.IntegrationTests.Api;

public class TransactionsControllerTests : IntegrationTestBase
{
    public TransactionsControllerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task CreateTransaction_WithValidData_ReturnsCreatedTransaction()
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
        var stopwatch = Stopwatch.StartNew();
        var response = await PostAsync($"/api/projects/{project.Id}/transactions", request);
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);

        var transaction = await response.Content.ReadFromJsonAsync<TransactionResponse>(JsonOptions);
        transaction.Should().NotBeNull();
        transaction!.Amount.Should().Be(request.Amount);
        transaction.Classification.Should().Be(request.Classification);
        transaction.DigitalSignature.Should().NotBeNullOrEmpty();
        transaction.DataHash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateTransaction_OnClosedProject_ReturnsBadRequest()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateTestCoordinator();
        var project = TestDataBuilder.CreateProject(coordinator, status: ProjectStatus.Completed);
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
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTransactionHistory_ReturnsAllTransactions()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        var project = TestDataBuilder.CreateProject(coordinator);
        var bankAccount = TestDataBuilder.CreateBankAccount(project);
        var accountingAccount = TestDataBuilder.CreateAccountingAccount();
        var transaction1 = TestDataBuilder.CreateTransaction(bankAccount, accountingAccount, 1000m, TransactionClassification.Debit);
        var transaction2 = TestDataBuilder.CreateTransaction(bankAccount, accountingAccount, 500m, TransactionClassification.Credit);

        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddAsync(project);
        await DbContext.BankAccounts.AddAsync(bankAccount);
        await DbContext.AccountingAccounts.AddAsync(accountingAccount);
        await DbContext.Transactions.AddRangeAsync(transaction1, transaction2);
        await DbContext.SaveChangesAsync();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await GetAsync<List<TransactionResponse>>($"/api/projects/{project.Id}/transactions");
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result!.Count.Should().Be(2);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
    }

    [Fact]
    public async Task GetAccountBalance_ReturnsCorrectBalance()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        var project = TestDataBuilder.CreateProject(coordinator, budgetAmount: 10000m);
        var bankAccount = TestDataBuilder.CreateBankAccount(project);
        var accountingAccount = TestDataBuilder.CreateAccountingAccount();
        
        // Create transactions: 5000 credit - 2000 debit = 3000 balance
        var credit = TestDataBuilder.CreateTransaction(bankAccount, accountingAccount, 5000m, TransactionClassification.Credit);
        var debit = TestDataBuilder.CreateTransaction(bankAccount, accountingAccount, 2000m, TransactionClassification.Debit);

        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddAsync(project);
        await DbContext.BankAccounts.AddAsync(bankAccount);
        await DbContext.AccountingAccounts.AddAsync(accountingAccount);
        await DbContext.Transactions.AddRangeAsync(credit, debit);
        await DbContext.SaveChangesAsync();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await GetAsync<AccountBalanceResponse>($"/api/projects/{project.Id}/balance");
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result!.Balance.Should().Be(3000m);
        result.TotalCredits.Should().Be(5000m);
        result.TotalDebits.Should().Be(2000m);
        result.TransactionCount.Should().Be(2);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
    }

    [Fact]
    public async Task CreateTransaction_WithInvalidAmount_ReturnsBadRequest()
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
            Amount = -1000m, // Invalid negative amount
            Date = DateTime.UtcNow.Date,
            Classification = "Debit",
            AccountingAccountId = accountingAccount.Id
        };

        // Act
        var response = await PostAsync($"/api/projects/{project.Id}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTransaction_WithFutureDate_ReturnsBadRequest()
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
            Amount = 1000m,
            Date = DateTime.UtcNow.Date.AddDays(1), // Future date
            Classification = "Debit",
            AccountingAccountId = accountingAccount.Id
        };

        // Act
        var response = await PostAsync($"/api/projects/{project.Id}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTransaction_CreatesAuditTrailEntry()
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
        var auditEntries = await DbContext.AuditEntries
            .Where(a => a.EntityId == transaction!.Id)
            .ToListAsync();

        auditEntries.Should().NotBeEmpty();
        auditEntries.Should().Contain(a => a.ActionType == "Create" && a.EntityType == "Transaction");
    }
}
