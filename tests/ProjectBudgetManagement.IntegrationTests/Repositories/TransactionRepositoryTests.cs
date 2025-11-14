using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.Infrastructure.Repositories;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;

namespace ProjectBudgetManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for TransactionRepository with real SQL Server database.
/// </summary>
public class TransactionRepositoryTests : IntegrationTestBase
{
    private readonly ITransactionRepository _repository;

    public TransactionRepositoryTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _repository = new TransactionRepository(DbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistTransactionToDatabase()
    {
        // Arrange
        var (coordinator, project, bankAccount, account) = await SetupProjectWithAccountsAsync();
        var transaction = TestDataBuilder.CreateTransaction(bankAccount, account, 5000m, TransactionClassification.Credit);

        // Act
        await _repository.AddAsync(transaction);
        await _repository.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetByIdAsync(transaction.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(transaction.Amount, retrieved.Amount);
        Assert.Equal(transaction.Classification, retrieved.Classification);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeRelatedEntities()
    {
        // Arrange
        var (coordinator, project, bankAccount, account) = await SetupProjectWithAccountsAsync();
        var transaction = TestDataBuilder.CreateTransaction(bankAccount, account);
        await SeedAsync(transaction);

        // Act
        var retrieved = await _repository.GetByIdAsync(transaction.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.NotNull(retrieved.BankAccount);
        Assert.NotNull(retrieved.AccountingAccount);
        Assert.Equal(account.Name, retrieved.AccountingAccount.Name);
    }

    [Fact]
    public async Task GetByBankAccountAsync_ShouldReturnAllTransactionsForAccount()
    {
        // Arrange
        var (coordinator, project, bankAccount, account) = await SetupProjectWithAccountsAsync();
        var transaction1 = TestDataBuilder.CreateTransaction(bankAccount, account, 1000m, TransactionClassification.Credit);
        var transaction2 = TestDataBuilder.CreateTransaction(bankAccount, account, 500m, TransactionClassification.Debit);
        await SeedAsync(transaction1, transaction2);

        // Act
        var transactions = await _repository.GetByBankAccountAsync(bankAccount.Id);

        // Assert
        Assert.True(transactions.Count >= 2);
        Assert.Contains(transactions, t => t.Id == transaction1.Id);
        Assert.Contains(transactions, t => t.Id == transaction2.Id);
    }

    [Fact]
    public async Task GetByBankAccountAsync_WithDateFilter_ShouldReturnFilteredTransactions()
    {
        // Arrange
        var (coordinator, project, bankAccount, account) = await SetupProjectWithAccountsAsync();
        var oldDate = DateTime.UtcNow.Date.AddDays(-30);
        var recentDate = DateTime.UtcNow.Date.AddDays(-5);
        
        var oldTransaction = TestDataBuilder.CreateTransactionWithDate(bankAccount, account, oldDate, 1000m);
        var recentTransaction = TestDataBuilder.CreateTransactionWithDate(bankAccount, account, recentDate, 500m);
        await SeedAsync(oldTransaction, recentTransaction);

        // Act
        var filteredTransactions = await _repository.GetByBankAccountAsync(
            bankAccount.Id,
            startDate: DateTime.UtcNow.Date.AddDays(-10));

        // Assert
        Assert.Contains(filteredTransactions, t => t.Id == recentTransaction.Id);
        Assert.DoesNotContain(filteredTransactions, t => t.Id == oldTransaction.Id);
    }

    [Fact]
    public async Task GetByBankAccountAsync_WithClassificationFilter_ShouldReturnFilteredTransactions()
    {
        // Arrange
        var (coordinator, project, bankAccount, account) = await SetupProjectWithAccountsAsync();
        var creditTransaction = TestDataBuilder.CreateTransaction(bankAccount, account, 1000m, TransactionClassification.Credit);
        var debitTransaction = TestDataBuilder.CreateTransaction(bankAccount, account, 500m, TransactionClassification.Debit);
        await SeedAsync(creditTransaction, debitTransaction);

        // Act
        var creditTransactions = await _repository.GetByBankAccountAsync(
            bankAccount.Id,
            classification: TransactionClassification.Credit);

        // Assert
        Assert.All(creditTransactions, t => Assert.Equal(TransactionClassification.Credit, t.Classification));
        Assert.Contains(creditTransactions, t => t.Id == creditTransaction.Id);
        Assert.DoesNotContain(creditTransactions, t => t.Id == debitTransaction.Id);
    }

    [Fact]
    public async Task GetByBankAccountAsync_WithAccountingAccountFilter_ShouldReturnFilteredTransactions()
    {
        // Arrange
        var (coordinator, project, bankAccount, account1) = await SetupProjectWithAccountsAsync();
        var account2 = TestDataBuilder.CreateUniqueAccountingAccount("Travel Expenses");
        await SeedAsync(account2);
        
        var transaction1 = TestDataBuilder.CreateTransaction(bankAccount, account1, 1000m);
        var transaction2 = TestDataBuilder.CreateTransaction(bankAccount, account2, 500m);
        await SeedAsync(transaction1, transaction2);

        // Act
        var filteredTransactions = await _repository.GetByBankAccountAsync(
            bankAccount.Id,
            accountingAccountId: account1.Id);

        // Assert
        Assert.All(filteredTransactions, t => Assert.Equal(account1.Id, t.AccountingAccountId));
        Assert.Contains(filteredTransactions, t => t.Id == transaction1.Id);
        Assert.DoesNotContain(filteredTransactions, t => t.Id == transaction2.Id);
    }

    [Fact]
    public async Task GetByBankAccountAsync_ShouldReturnTransactionsInChronologicalOrder()
    {
        // Arrange
        var (coordinator, project, bankAccount, account) = await SetupProjectWithAccountsAsync();
        var date1 = DateTime.UtcNow.Date.AddDays(-10);
        var date2 = DateTime.UtcNow.Date.AddDays(-5);
        var date3 = DateTime.UtcNow.Date;
        
        var transaction1 = TestDataBuilder.CreateTransactionWithDate(bankAccount, account, date1, 1000m);
        var transaction2 = TestDataBuilder.CreateTransactionWithDate(bankAccount, account, date2, 500m);
        var transaction3 = TestDataBuilder.CreateTransactionWithDate(bankAccount, account, date3, 750m);
        await SeedAsync(transaction3, transaction1, transaction2); // Insert in random order

        // Act
        var transactions = await _repository.GetByBankAccountAsync(bankAccount.Id);

        // Assert
        Assert.True(transactions.Count >= 3);
        var orderedTransactions = transactions.Where(t => 
            t.Id == transaction1.Id || t.Id == transaction2.Id || t.Id == transaction3.Id).ToList();
        
        for (int i = 0; i < orderedTransactions.Count - 1; i++)
        {
            Assert.True(orderedTransactions[i].Date <= orderedTransactions[i + 1].Date);
        }
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnNumberOfAffectedEntities()
    {
        // Arrange
        var (coordinator, project, bankAccount, account) = await SetupProjectWithAccountsAsync();
        var transaction = TestDataBuilder.CreateTransaction(bankAccount, account);
        await _repository.AddAsync(transaction);

        // Act
        var affectedRows = await _repository.SaveChangesAsync();

        // Assert
        Assert.Equal(1, affectedRows);
    }

    private async Task<(Person coordinator, Project project, BankAccount bankAccount, AccountingAccount account)> SetupProjectWithAccountsAsync()
    {
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project = TestDataBuilder.CreateUniqueProject(coordinator);
        var bankAccount = TestDataBuilder.CreateUniqueBankAccount(project);
        var account = TestDataBuilder.CreateUniqueAccountingAccount();
        
        await SeedAsync(coordinator, project, bankAccount, account);
        
        return (coordinator, project, bankAccount, account);
    }
}
