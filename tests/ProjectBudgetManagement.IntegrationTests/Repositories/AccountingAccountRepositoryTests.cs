using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Infrastructure.Repositories;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;

namespace ProjectBudgetManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for AccountingAccountRepository with real SQL Server database.
/// </summary>
public class AccountingAccountRepositoryTests : IntegrationTestBase
{
    private readonly IAccountingAccountRepository _repository;

    public AccountingAccountRepositoryTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _repository = new AccountingAccountRepository(DbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistAccountToDatabase()
    {
        // Arrange
        var account = TestDataBuilder.CreateUniqueAccountingAccount("Office Supplies");

        // Act
        await _repository.AddAsync(account);
        await _repository.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetByIdAsync(account.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(account.Name, retrieved.Name);
        Assert.Equal(account.Identifier, retrieved.Identifier);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectAccount()
    {
        // Arrange
        var account = TestDataBuilder.CreateUniqueAccountingAccount();
        await SeedAsync(account);

        // Act
        var retrieved = await _repository.GetByIdAsync(account.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(account.Id, retrieved.Id);
        Assert.Equal(account.Name, retrieved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Act
        var retrieved = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task GetByIdentifierAsync_ShouldReturnCorrectAccount()
    {
        // Arrange
        var account = TestDataBuilder.CreateUniqueAccountingAccount();
        await SeedAsync(account);

        // Act
        var retrieved = await _repository.GetByIdentifierAsync(account.Identifier);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(account.Id, retrieved.Id);
        Assert.Equal(account.Identifier, retrieved.Identifier);
    }

    [Fact]
    public async Task GetByIdentifierAsync_WithNonExistentIdentifier_ShouldReturnNull()
    {
        // Act
        var retrieved = await _repository.GetByIdentifierAsync("9999.99.9999");

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAccounts()
    {
        // Arrange
        var account1 = TestDataBuilder.CreateUniqueAccountingAccount("Account 1");
        var account2 = TestDataBuilder.CreateUniqueAccountingAccount("Account 2");
        await SeedAsync(account1, account2);

        // Act
        var accounts = await _repository.GetAllAsync();

        // Assert
        Assert.True(accounts.Count >= 2);
        Assert.Contains(accounts, a => a.Id == account1.Id);
        Assert.Contains(accounts, a => a.Id == account2.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAccountsOrderedByIdentifier()
    {
        // Arrange
        var account1 = TestDataBuilder.CreateAccountingAccount("Account A", "1001.01.0001");
        var account2 = TestDataBuilder.CreateAccountingAccount("Account B", "1002.01.0001");
        var account3 = TestDataBuilder.CreateAccountingAccount("Account C", "1000.01.0001");
        await SeedAsync(account1, account2, account3);

        // Act
        var accounts = await _repository.GetAllAsync();

        // Assert
        var testAccounts = accounts.Where(a => 
            a.Id == account1.Id || a.Id == account2.Id || a.Id == account3.Id).ToList();
        
        Assert.Equal(3, testAccounts.Count);
        Assert.Equal(account3.Id, testAccounts[0].Id); // 1000.01.0001
        Assert.Equal(account1.Id, testAccounts[1].Id); // 1001.01.0001
        Assert.Equal(account2.Id, testAccounts[2].Id); // 1002.01.0001
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnNumberOfAffectedEntities()
    {
        // Arrange
        var account = TestDataBuilder.CreateUniqueAccountingAccount();
        await _repository.AddAsync(account);

        // Act
        var affectedRows = await _repository.SaveChangesAsync();

        // Assert
        Assert.Equal(1, affectedRows);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateIdentifier_ShouldThrowException()
    {
        // Arrange
        var identifier = "1001.01.0001";
        var account1 = TestDataBuilder.CreateAccountingAccount("Account 1", identifier);
        var account2 = TestDataBuilder.CreateAccountingAccount("Account 2", identifier);
        
        await _repository.AddAsync(account1);
        await _repository.SaveChangesAsync();

        // Act & Assert
        await _repository.AddAsync(account2);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await _repository.SaveChangesAsync());
    }
}
