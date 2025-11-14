using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.IntegrationTests.Builders;

/// <summary>
/// Test data builder providing factory methods for creating test entities with realistic data.
/// </summary>
public class TestDataBuilder
{
    // Test user ID that matches the one used in authorization
    public static readonly Guid TestUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    
    private static int _personCounter = 0;
    private static int _projectCounter = 0;
    private static int _accountCounter = 0;

    #region Person Builders

    public static Person CreatePerson(string name = "John Doe", string identificationNumber = "12345678900", Guid? id = null)
    {
        return new Person
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            IdentificationNumber = identificationNumber,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Person CreateTestCoordinator()
    {
        return CreatePerson("Test Coordinator", "12345678900", TestUserId);
    }

    /// <summary>
    /// Creates a person with unique identification number to avoid conflicts.
    /// </summary>
    public static Person CreateUniquePerson(string? name = null)
    {
        var counter = Interlocked.Increment(ref _personCounter);
        return CreatePerson(
            name ?? $"Person {counter}",
            $"ID{counter:D11}" // 11 digits for Brazilian CPF format
        );
    }

    #endregion

    #region Project Builders

    public static Project CreateProject(
        Person coordinator,
        string name = "Test Project",
        decimal budgetAmount = 100000m,
        ProjectStatus status = ProjectStatus.NotStarted)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = "Test project description",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddMonths(6),
            Status = status,
            BudgetAmount = budgetAmount,
            CoordinatorId = coordinator.Id,
            Coordinator = coordinator,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return project;
    }

    /// <summary>
    /// Creates a project with unique name and realistic data.
    /// </summary>
    public static Project CreateUniqueProject(Person coordinator, ProjectStatus status = ProjectStatus.NotStarted)
    {
        var counter = Interlocked.Increment(ref _projectCounter);
        return CreateProject(
            coordinator,
            $"Project {counter}",
            50000m + (counter * 10000m), // Varying budget amounts
            status
        );
    }

    /// <summary>
    /// Creates a project with custom date range.
    /// </summary>
    public static Project CreateProjectWithDates(
        Person coordinator,
        DateTime startDate,
        DateTime endDate,
        string name = "Test Project",
        decimal budgetAmount = 100000m)
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = $"Project running from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
            StartDate = startDate,
            EndDate = endDate,
            Status = ProjectStatus.NotStarted,
            BudgetAmount = budgetAmount,
            CoordinatorId = coordinator.Id,
            Coordinator = coordinator,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    #endregion

    #region BankAccount Builders

    public static BankAccount CreateBankAccount(
        Project project,
        string accountNumber = "12345678",
        string bankName = "Test Bank",
        string branchNumber = "0001")
    {
        return new BankAccount
        {
            Id = Guid.NewGuid(),
            AccountNumber = accountNumber,
            BankName = bankName,
            BranchNumber = branchNumber,
            AccountHolderName = project.Coordinator?.Name ?? "Unknown",
            ProjectId = project.Id,
            Project = project,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a bank account with unique account number to avoid conflicts.
    /// </summary>
    public static BankAccount CreateUniqueBankAccount(Project project, string? bankName = null)
    {
        var counter = Interlocked.Increment(ref _accountCounter);
        return CreateBankAccount(
            project,
            $"{counter:D8}", // 8-digit account number
            bankName ?? "Test Bank",
            $"{(counter % 9999):D4}" // 4-digit branch number
        );
    }

    #endregion

    #region AccountingAccount Builders

    public static AccountingAccount CreateAccountingAccount(
        string name = "Office Supplies",
        string identifier = "1001.01.0001")
    {
        return new AccountingAccount
        {
            Id = Guid.NewGuid(),
            Name = name,
            Identifier = identifier,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an accounting account with unique identifier.
    /// </summary>
    public static AccountingAccount CreateUniqueAccountingAccount(string? name = null)
    {
        var counter = Interlocked.Increment(ref _accountCounter);
        return CreateAccountingAccount(
            name ?? $"Account {counter}",
            $"{1000 + counter}.01.{counter:D4}"
        );
    }

    /// <summary>
    /// Creates a set of standard accounting accounts for testing.
    /// </summary>
    public static List<AccountingAccount> CreateStandardAccountingAccounts()
    {
        return new List<AccountingAccount>
        {
            CreateAccountingAccount("Office Supplies", "1001.01.0001"),
            CreateAccountingAccount("Equipment", "1001.02.0001"),
            CreateAccountingAccount("Travel Expenses", "1002.01.0001"),
            CreateAccountingAccount("Consulting Services", "1003.01.0001"),
            CreateAccountingAccount("Software Licenses", "1003.02.0001")
        };
    }

    #endregion

    #region Transaction Builders

    public static Transaction CreateTransaction(
        BankAccount bankAccount,
        AccountingAccount accountingAccount,
        decimal amount = 1000m,
        TransactionClassification classification = TransactionClassification.Debit,
        Guid? createdBy = null)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Date = DateTime.UtcNow.Date,
            Classification = classification,
            DigitalSignature = $"SIGNATURE_{Guid.NewGuid()}",
            DataHash = $"HASH_{Guid.NewGuid()}",
            BankAccountId = bankAccount.Id,
            BankAccount = bankAccount,
            AccountingAccountId = accountingAccount.Id,
            AccountingAccount = accountingAccount,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy ?? TestUserId
        };
    }

    /// <summary>
    /// Creates a transaction with specific date.
    /// </summary>
    public static Transaction CreateTransactionWithDate(
        BankAccount bankAccount,
        AccountingAccount accountingAccount,
        DateTime date,
        decimal amount = 1000m,
        TransactionClassification classification = TransactionClassification.Debit)
    {
        var transaction = CreateTransaction(bankAccount, accountingAccount, amount, classification);
        transaction.Date = date;
        return transaction;
    }

    /// <summary>
    /// Creates multiple transactions for testing balance calculations.
    /// </summary>
    public static List<Transaction> CreateTransactionSet(
        BankAccount bankAccount,
        AccountingAccount accountingAccount)
    {
        return new List<Transaction>
        {
            CreateTransaction(bankAccount, accountingAccount, 10000m, TransactionClassification.Credit), // Initial deposit
            CreateTransaction(bankAccount, accountingAccount, 2500m, TransactionClassification.Debit),   // Expense 1
            CreateTransaction(bankAccount, accountingAccount, 1500m, TransactionClassification.Debit),   // Expense 2
            CreateTransaction(bankAccount, accountingAccount, 5000m, TransactionClassification.Credit),  // Additional funding
            CreateTransaction(bankAccount, accountingAccount, 3000m, TransactionClassification.Debit)    // Expense 3
        };
        // Expected balance: 10000 - 2500 - 1500 + 5000 - 3000 = 8000
    }

    #endregion

    #region Complete Test Scenarios

    /// <summary>
    /// Creates a complete project setup with coordinator, project, bank account, and accounting accounts.
    /// </summary>
    public static (Person coordinator, Project project, BankAccount bankAccount, List<AccountingAccount> accounts) CreateCompleteProjectSetup()
    {
        var coordinator = CreateUniquePerson("Project Coordinator");
        var project = CreateUniqueProject(coordinator);
        var bankAccount = CreateUniqueBankAccount(project);
        var accounts = CreateStandardAccountingAccounts();

        return (coordinator, project, bankAccount, accounts);
    }

    /// <summary>
    /// Creates a project with transactions for balance testing.
    /// </summary>
    public static (Person coordinator, Project project, BankAccount bankAccount, AccountingAccount account, List<Transaction> transactions) CreateProjectWithTransactions()
    {
        var coordinator = CreateUniquePerson();
        var project = CreateUniqueProject(coordinator);
        var bankAccount = CreateUniqueBankAccount(project);
        var account = CreateUniqueAccountingAccount();
        var transactions = CreateTransactionSet(bankAccount, account);

        return (coordinator, project, bankAccount, account, transactions);
    }

    #endregion

    #region Counter Reset (for test isolation)

    /// <summary>
    /// Resets all counters. Use with caution - only for test cleanup.
    /// </summary>
    public static void ResetCounters()
    {
        _personCounter = 0;
        _projectCounter = 0;
        _accountCounter = 0;
    }

    #endregion
}
