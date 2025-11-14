using FluentAssertions;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ProjectBudgetManagement.IntegrationTests.Api;

public class ErrorHandlingTests : IntegrationTestBase
{
    public ErrorHandlingTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task ValidationError_Returns400WithDetails()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            Name = "", // Empty name - validation error
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddMonths(1),
            BudgetAmount = 10000m,
            CoordinatorId = Guid.NewGuid()
        };

        // Act
        var response = await PostAsync("/api/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task NotFoundError_Returns404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/projects/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ConflictError_Returns409()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        var project = TestDataBuilder.CreateProject(coordinator);
        var bankAccount = TestDataBuilder.CreateBankAccount(project, "12345678", "Test Bank", "0001");

        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddAsync(project);
        await DbContext.BankAccounts.AddAsync(bankAccount);
        await DbContext.SaveChangesAsync();

        // Try to create another project with the same bank account
        var newCoordinator = TestDataBuilder.CreatePerson("Jane Doe", "98765432100");
        await DbContext.Persons.AddAsync(newCoordinator);
        await DbContext.SaveChangesAsync();

        var request = new CreateProjectRequest
        {
            Name = "Another Project",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddMonths(1),
            BudgetAmount = 10000m,
            CoordinatorId = newCoordinator.Id,
            BankAccount = new BankAccountRequest
            {
                AccountNumber = "12345678", // Duplicate
                BankName = "Test Bank",
                BranchNumber = "0001",
                AccountHolderName = newCoordinator.Name
            }
        };

        // Act
        var response = await PostAsync("/api/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task BusinessRuleViolation_Returns400()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.SaveChangesAsync();

        var request = new CreateProjectRequest
        {
            Name = "Test Project",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(-1), // End date before start date
            BudgetAmount = 10000m,
            CoordinatorId = coordinator.Id,
            BankAccount = new BankAccountRequest
            {
                AccountNumber = "12345678",
                BankName = "Test Bank",
                BranchNumber = "0001",
                AccountHolderName = coordinator.Name
            }
        };

        // Act
        var response = await PostAsync("/api/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task NegativeBudgetAmount_Returns400()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.SaveChangesAsync();

        var request = new CreateProjectRequest
        {
            Name = "Test Project",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddMonths(1),
            BudgetAmount = -1000m, // Negative budget
            CoordinatorId = coordinator.Id
        };

        // Act
        var response = await PostAsync("/api/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task TransactionOnCompletedProject_Returns400()
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
            Amount = 1000m,
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
    public async Task TransactionOnCancelledProject_Returns400()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateTestCoordinator();
        var project = TestDataBuilder.CreateProject(coordinator, status: ProjectStatus.Cancelled);
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
    public async Task InvalidTransactionClassification_Returns400()
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
            Date = DateTime.UtcNow.Date,
            Classification = "InvalidType", // Invalid classification
            AccountingAccountId = accountingAccount.Id
        };

        // Act
        var response = await PostAsync($"/api/projects/{project.Id}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
