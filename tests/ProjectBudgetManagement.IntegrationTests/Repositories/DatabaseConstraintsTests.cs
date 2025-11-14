using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.Infrastructure.Repositories;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;

namespace ProjectBudgetManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for database constraints enforcement and transaction rollback scenarios.
/// </summary>
public class DatabaseConstraintsTests : IntegrationTestBase
{
    public DatabaseConstraintsTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    #region Unique Constraint Tests

    [Fact]
    public async Task BankAccount_WithDuplicateAccountDetails_ShouldViolateUniqueConstraint()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project1 = TestDataBuilder.CreateUniqueProject(coordinator);
        var project2 = TestDataBuilder.CreateUniqueProject(coordinator);
        
        var bankAccount1 = TestDataBuilder.CreateBankAccount(project1, "12345678", "Test Bank", "0001");
        var bankAccount2 = TestDataBuilder.CreateBankAccount(project2, "12345678", "Test Bank", "0001");
        
        await SeedAsync(coordinator, project1, project2, bankAccount1);

        // Act & Assert
        await DbContext.BankAccounts.AddAsync(bankAccount2);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task Person_WithDuplicateIdentificationNumber_ShouldViolateUniqueConstraint()
    {
        // Arrange
        var person1 = TestDataBuilder.CreatePerson("Person 1", "12345678900");
        var person2 = TestDataBuilder.CreatePerson("Person 2", "12345678900");
        
        await SeedAsync(person1);

        // Act & Assert
        await DbContext.Persons.AddAsync(person2);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task AccountingAccount_WithDuplicateIdentifier_ShouldViolateUniqueConstraint()
    {
        // Arrange
        var account1 = TestDataBuilder.CreateAccountingAccount("Account 1", "1001.01.0001");
        var account2 = TestDataBuilder.CreateAccountingAccount("Account 2", "1001.01.0001");
        
        await SeedAsync(account1);

        // Act & Assert
        await DbContext.AccountingAccounts.AddAsync(account2);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task Project_WithOneToOneBankAccount_ShouldEnforceUniqueRelationship()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project1 = TestDataBuilder.CreateUniqueProject(coordinator);
        var project2 = TestDataBuilder.CreateUniqueProject(coordinator);
        var bankAccount1 = TestDataBuilder.CreateUniqueBankAccount(project1);
        
        await SeedAsync(coordinator, project1, project2, bankAccount1);

        // Create second bank account with same ProjectId but different account details
        var bankAccount2 = new BankAccount
        {
            Id = Guid.NewGuid(),
            AccountNumber = "99999999",
            BankName = "Different Bank",
            BranchNumber = "9999",
            AccountHolderName = coordinator.Name,
            ProjectId = project1.Id, // Same project as bankAccount1 - should violate unique constraint
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert - Second bank account for same project should fail
        DbContext.ChangeTracker.Clear(); // Clear tracking to avoid conflicts
        await DbContext.BankAccounts.AddAsync(bankAccount2);
        var exception = await Assert.ThrowsAnyAsync<Exception>(async () => 
            await DbContext.SaveChangesAsync());
        
        // Verify it's a database constraint violation
        Assert.True(exception is DbUpdateException || exception.InnerException is DbUpdateException,
            $"Expected DbUpdateException but got {exception.GetType().Name}: {exception.Message}");
    }

    #endregion

    #region Check Constraint Tests

    [Fact]
    public async Task Project_WithStartDateAfterEndDate_ShouldViolateCheckConstraint()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        await SeedAsync(coordinator);
        
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Invalid Project",
            Description = "Test",
            StartDate = DateTime.UtcNow.Date.AddDays(10),
            EndDate = DateTime.UtcNow.Date, // End before start
            Status = ProjectStatus.NotStarted,
            BudgetAmount = 100000m,
            CoordinatorId = coordinator.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await DbContext.Projects.AddAsync(project);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task Project_WithNegativeBudget_ShouldViolateCheckConstraint()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        await SeedAsync(coordinator);
        
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Invalid Project",
            Description = "Test",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddMonths(6),
            Status = ProjectStatus.NotStarted,
            BudgetAmount = -1000m, // Negative budget
            CoordinatorId = coordinator.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await DbContext.Projects.AddAsync(project);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task Transaction_WithNegativeAmount_ShouldViolateCheckConstraint()
    {
        // Arrange
        var (coordinator, project, bankAccount, account) = await SetupProjectWithAccountsAsync();
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = -500m, // Negative amount
            Date = DateTime.UtcNow.Date,
            Classification = TransactionClassification.Debit,
            DigitalSignature = "SIGNATURE",
            DataHash = "HASH",
            BankAccountId = bankAccount.Id,
            AccountingAccountId = account.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = TestUserId
        };

        // Act & Assert
        await DbContext.Transactions.AddAsync(transaction);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    #endregion

    #region Foreign Key Constraint Tests

    [Fact]
    public async Task Project_WithNonExistentCoordinator_ShouldViolateForeignKeyConstraint()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Invalid Project",
            Description = "Test",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddMonths(6),
            Status = ProjectStatus.NotStarted,
            BudgetAmount = 100000m,
            CoordinatorId = Guid.NewGuid(), // Non-existent coordinator
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await DbContext.Projects.AddAsync(project);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task BankAccount_WithNonExistentProject_ShouldViolateForeignKeyConstraint()
    {
        // Arrange
        var bankAccount = new BankAccount
        {
            Id = Guid.NewGuid(),
            AccountNumber = "12345678",
            BankName = "Test Bank",
            BranchNumber = "0001",
            AccountHolderName = "Test Holder",
            ProjectId = Guid.NewGuid(), // Non-existent project
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await DbContext.BankAccounts.AddAsync(bankAccount);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task Transaction_WithNonExistentBankAccount_ShouldViolateForeignKeyConstraint()
    {
        // Arrange
        var account = TestDataBuilder.CreateUniqueAccountingAccount();
        await SeedAsync(account);
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = 1000m,
            Date = DateTime.UtcNow.Date,
            Classification = TransactionClassification.Debit,
            DigitalSignature = "SIGNATURE",
            DataHash = "HASH",
            BankAccountId = Guid.NewGuid(), // Non-existent bank account
            AccountingAccountId = account.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = TestUserId
        };

        // Act & Assert
        await DbContext.Transactions.AddAsync(transaction);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task Transaction_WithNonExistentAccountingAccount_ShouldViolateForeignKeyConstraint()
    {
        // Arrange
        var (coordinator, project, bankAccount, _) = await SetupProjectWithAccountsAsync();
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = 1000m,
            Date = DateTime.UtcNow.Date,
            Classification = TransactionClassification.Debit,
            DigitalSignature = "SIGNATURE",
            DataHash = "HASH",
            BankAccountId = bankAccount.Id,
            AccountingAccountId = Guid.NewGuid(), // Non-existent accounting account
            CreatedAt = DateTime.UtcNow,
            CreatedBy = TestUserId
        };

        // Act & Assert
        await DbContext.Transactions.AddAsync(transaction);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await DbContext.SaveChangesAsync());
    }

    #endregion

    #region Transaction Rollback Tests

    [Fact]
    public async Task Transaction_WhenExceptionOccurs_ShouldRollbackAllChanges()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project = TestDataBuilder.CreateUniqueProject(coordinator);
        
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        
        try
        {
            // Act - Add valid entities
            await DbContext.Persons.AddAsync(coordinator);
            await DbContext.Projects.AddAsync(project);
            await DbContext.SaveChangesAsync();
            
            // Add invalid entity to cause rollback
            var invalidProject = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Invalid",
                Description = "Test",
                StartDate = DateTime.UtcNow.Date.AddDays(10),
                EndDate = DateTime.UtcNow.Date, // Invalid: end before start
                Status = ProjectStatus.NotStarted,
                BudgetAmount = 100000m,
                CoordinatorId = coordinator.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await DbContext.Projects.AddAsync(invalidProject);
            await DbContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
            Assert.Fail("Should have thrown exception");
        }
        catch (DbUpdateException)
        {
            // Expected exception
            await transaction.RollbackAsync();
        }
        
        // Assert - Verify rollback occurred
        DbContext.ChangeTracker.Clear();
        var retrievedPerson = await DbContext.Persons.FindAsync(coordinator.Id);
        var retrievedProject = await DbContext.Projects.FindAsync(project.Id);
        
        Assert.Null(retrievedPerson);
        Assert.Null(retrievedProject);
    }

    [Fact]
    public async Task Transaction_WithMultipleOperations_ShouldCommitAllOrNone()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project = TestDataBuilder.CreateUniqueProject(coordinator);
        var bankAccount = TestDataBuilder.CreateUniqueBankAccount(project);
        
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        
        // Act
        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddAsync(project);
        await DbContext.BankAccounts.AddAsync(bankAccount);
        await DbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        
        // Assert - All entities should be persisted
        DbContext.ChangeTracker.Clear();
        var retrievedPerson = await DbContext.Persons.FindAsync(coordinator.Id);
        var retrievedProject = await DbContext.Projects.FindAsync(project.Id);
        var retrievedBankAccount = await DbContext.BankAccounts.FindAsync(bankAccount.Id);
        
        Assert.NotNull(retrievedPerson);
        Assert.NotNull(retrievedProject);
        Assert.NotNull(retrievedBankAccount);
    }

    [Fact]
    public async Task Transaction_WithExplicitRollback_ShouldNotPersistChanges()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project = TestDataBuilder.CreateUniqueProject(coordinator);
        
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        
        // Act
        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddAsync(project);
        await DbContext.SaveChangesAsync();
        
        // Explicitly rollback
        await transaction.RollbackAsync();
        
        // Assert - Changes should not be persisted
        DbContext.ChangeTracker.Clear();
        var retrievedPerson = await DbContext.Persons.FindAsync(coordinator.Id);
        var retrievedProject = await DbContext.Projects.FindAsync(project.Id);
        
        Assert.Null(retrievedPerson);
        Assert.Null(retrievedProject);
    }

    #endregion

    #region Cascade Delete Tests

    [Fact]
    public async Task DeleteProject_ShouldCascadeDeleteBankAccount()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project = TestDataBuilder.CreateUniqueProject(coordinator);
        var bankAccount = TestDataBuilder.CreateUniqueBankAccount(project);
        await SeedAsync(coordinator, project, bankAccount);

        // Act
        DbContext.Projects.Remove(project);
        await DbContext.SaveChangesAsync();

        // Assert
        DbContext.ChangeTracker.Clear();
        var retrievedBankAccount = await DbContext.BankAccounts.FindAsync(bankAccount.Id);
        Assert.Null(retrievedBankAccount);
    }

    #endregion

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
