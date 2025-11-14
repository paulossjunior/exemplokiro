using FluentAssertions;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace ProjectBudgetManagement.IntegrationTests.Api;

public class AccountingAccountsControllerTests : IntegrationTestBase
{
    public AccountingAccountsControllerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task CreateAccountingAccount_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new CreateAccountingAccountRequest
        {
            Name = "Office Supplies",
            Identifier = "1001.01.0001"
        };

        // Act
        var stopwatch = Stopwatch.StartNew();
        var response = await PostAsync("/api/accounting-accounts", request);
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);

        var account = await response.Content.ReadFromJsonAsync<AccountingAccountResponse>(JsonOptions);
        account.Should().NotBeNull();
        account!.Name.Should().Be(request.Name);
        account.Identifier.Should().Be(request.Identifier);
    }

    [Fact]
    public async Task CreateAccountingAccount_WithDuplicateIdentifier_ReturnsConflict()
    {
        // Arrange
        var existingAccount = TestDataBuilder.CreateAccountingAccount("Existing", "1001.01.0001");
        await DbContext.AccountingAccounts.AddAsync(existingAccount);
        await DbContext.SaveChangesAsync();

        var request = new CreateAccountingAccountRequest
        {
            Name = "New Account",
            Identifier = "1001.01.0001" // Duplicate identifier
        };

        // Act
        var response = await PostAsync("/api/accounting-accounts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ListAccountingAccounts_ReturnsAllAccounts()
    {
        // Arrange
        var account1 = TestDataBuilder.CreateAccountingAccount("Account 1", "1001.01.0001");
        var account2 = TestDataBuilder.CreateAccountingAccount("Account 2", "1001.01.0002");

        await DbContext.AccountingAccounts.AddRangeAsync(account1, account2);
        await DbContext.SaveChangesAsync();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await GetAsync<List<AccountingAccountResponse>>("/api/accounting-accounts");
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterThanOrEqualTo(2);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500, "Integration tests with real database should complete reasonably fast");
    }

    [Fact]
    public async Task GetAccountingAccount_WithExistingId_ReturnsAccount()
    {
        // Arrange
        var account = TestDataBuilder.CreateAccountingAccount();
        await DbContext.AccountingAccounts.AddAsync(account);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await GetAsync<AccountingAccountResponse>($"/api/accounting-accounts/{account.Id}");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(account.Id);
        result.Name.Should().Be(account.Name);
    }

    [Fact]
    public async Task GetAccountingAccount_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/accounting-accounts/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAccountingAccount_WithInvalidIdentifierFormat_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountingAccountRequest
        {
            Name = "Test Account",
            Identifier = "INVALID_FORMAT" // Invalid format
        };

        // Act
        var response = await PostAsync("/api/accounting-accounts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
