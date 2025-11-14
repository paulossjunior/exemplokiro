using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.Infrastructure.Persistence;

namespace ProjectBudgetManagement.IntegrationTests.Builders;

/// <summary>
/// Provides methods for seeding realistic test data into the database.
/// </summary>
public class TestDataSeeder
{
    private readonly ProjectBudgetDbContext _context;

    public TestDataSeeder(ProjectBudgetDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seeds a complete project scenario with coordinator, project, bank account, and transactions.
    /// </summary>
    public async Task<ProjectScenario> SeedCompleteProjectScenario(
        string projectName = "Infrastructure Upgrade",
        decimal budget = 150000m,
        ProjectStatus status = ProjectStatus.InProgress)
    {
        var coordinator = TestDataBuilder.CreateUniquePerson("Maria Silva");
        var project = TestDataBuilder.CreateProject(coordinator, projectName, budget, status);
        var bankAccount = TestDataBuilder.CreateUniqueBankAccount(project, "Banco do Brasil");
        var accounts = TestDataBuilder.CreateStandardAccountingAccounts();

        await _context.Persons.AddAsync(coordinator);
        await _context.Projects.AddAsync(project);
        await _context.BankAccounts.AddAsync(bankAccount);
        await _context.AccountingAccounts.AddRangeAsync(accounts);
        await _context.SaveChangesAsync();

        // Create some transactions
        var transactions = new List<Transaction>
        {
            TestDataBuilder.CreateTransactionWithDate(
                bankAccount, accounts[0], DateTime.UtcNow.AddDays(-30), 15000m, TransactionClassification.Credit),
            TestDataBuilder.CreateTransactionWithDate(
                bankAccount, accounts[1], DateTime.UtcNow.AddDays(-25), 5000m, TransactionClassification.Debit),
            TestDataBuilder.CreateTransactionWithDate(
                bankAccount, accounts[2], DateTime.UtcNow.AddDays(-20), 2500m, TransactionClassification.Debit)
        };

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        return new ProjectScenario
        {
            Coordinator = coordinator,
            Project = project,
            BankAccount = bankAccount,
            AccountingAccounts = accounts,
            Transactions = transactions
        };
    }

    /// <summary>
    /// Seeds multiple projects with different statuses for testing.
    /// </summary>
    public async Task<List<ProjectScenario>> SeedMultipleProjects(int count = 3)
    {
        var scenarios = new List<ProjectScenario>();
        var statuses = new[] { ProjectStatus.NotStarted, ProjectStatus.InProgress, ProjectStatus.Completed };

        for (int i = 0; i < count; i++)
        {
            var status = statuses[i % statuses.Length];
            var scenario = await SeedCompleteProjectScenario(
                $"Project {i + 1}",
                50000m + (i * 25000m),
                status
            );
            scenarios.Add(scenario);
        }

        return scenarios;
    }

    /// <summary>
    /// Seeds a project with many transactions for performance testing.
    /// </summary>
    public async Task<ProjectScenario> SeedProjectWithManyTransactions(int transactionCount = 50)
    {
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project = TestDataBuilder.CreateUniqueProject(coordinator);
        var bankAccount = TestDataBuilder.CreateUniqueBankAccount(project);
        var account = TestDataBuilder.CreateUniqueAccountingAccount();

        await _context.Persons.AddAsync(coordinator);
        await _context.Projects.AddAsync(project);
        await _context.BankAccounts.AddAsync(bankAccount);
        await _context.AccountingAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        var transactions = new List<Transaction>();
        var random = new Random(42); // Fixed seed for reproducibility

        for (int i = 0; i < transactionCount; i++)
        {
            var classification = i % 3 == 0 ? TransactionClassification.Credit : TransactionClassification.Debit;
            var amount = random.Next(100, 5000);
            var date = DateTime.UtcNow.AddDays(-transactionCount + i);

            transactions.Add(TestDataBuilder.CreateTransactionWithDate(
                bankAccount, account, date, amount, classification));
        }

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        return new ProjectScenario
        {
            Coordinator = coordinator,
            Project = project,
            BankAccount = bankAccount,
            AccountingAccounts = new List<AccountingAccount> { account },
            Transactions = transactions
        };
    }

    /// <summary>
    /// Seeds audit entries for testing audit trail functionality.
    /// </summary>
    public async Task<List<AuditEntry>> SeedAuditEntries(Guid entityId, string entityType, int count = 5)
    {
        var entries = new List<AuditEntry>();
        var actions = new[] { "Created", "Updated", "StatusChanged", "Deleted" };

        for (int i = 0; i < count; i++)
        {
            var entry = new AuditEntry
            {
                Id = Guid.NewGuid(),
                UserId = TestDataBuilder.TestUserId,
                ActionType = actions[i % actions.Length],
                EntityType = entityType,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow.AddMinutes(-count + i),
                PreviousValue = i > 0 ? $"{{\"status\": \"Status{i - 1}\"}}" : null,
                NewValue = $"{{\"status\": \"Status{i}\"}}",
                DigitalSignature = $"SIGNATURE_{Guid.NewGuid()}",
                DataHash = $"HASH_{Guid.NewGuid()}"
            };
            entries.Add(entry);
        }

        await _context.AuditEntries.AddRangeAsync(entries);
        await _context.SaveChangesAsync();

        return entries;
    }

    /// <summary>
    /// Seeds a project at budget limit for testing budget warnings.
    /// </summary>
    public async Task<ProjectScenario> SeedProjectAtBudgetLimit()
    {
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var budget = 10000m;
        var project = TestDataBuilder.CreateProject(coordinator, "Budget Limited Project", budget);
        var bankAccount = TestDataBuilder.CreateUniqueBankAccount(project);
        var account = TestDataBuilder.CreateUniqueAccountingAccount();

        await _context.Persons.AddAsync(coordinator);
        await _context.Projects.AddAsync(project);
        await _context.BankAccounts.AddAsync(bankAccount);
        await _context.AccountingAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        // Create transactions that nearly exhaust the budget
        var transactions = new List<Transaction>
        {
            TestDataBuilder.CreateTransaction(bankAccount, account, 10000m, TransactionClassification.Credit),
            TestDataBuilder.CreateTransaction(bankAccount, account, 9500m, TransactionClassification.Debit)
        };
        // Balance: 500 remaining out of 10000 budget

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        return new ProjectScenario
        {
            Coordinator = coordinator,
            Project = project,
            BankAccount = bankAccount,
            AccountingAccounts = new List<AccountingAccount> { account },
            Transactions = transactions
        };
    }

    /// <summary>
    /// Seeds a project with transactions exceeding budget for testing warnings.
    /// </summary>
    public async Task<ProjectScenario> SeedProjectOverBudget()
    {
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var budget = 5000m;
        var project = TestDataBuilder.CreateProject(coordinator, "Over Budget Project", budget);
        var bankAccount = TestDataBuilder.CreateUniqueBankAccount(project);
        var account = TestDataBuilder.CreateUniqueAccountingAccount();

        await _context.Persons.AddAsync(coordinator);
        await _context.Projects.AddAsync(project);
        await _context.BankAccounts.AddAsync(bankAccount);
        await _context.AccountingAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        var transactions = new List<Transaction>
        {
            TestDataBuilder.CreateTransaction(bankAccount, account, 5000m, TransactionClassification.Credit),
            TestDataBuilder.CreateTransaction(bankAccount, account, 6000m, TransactionClassification.Debit)
        };
        // Balance: -1000 (over budget)

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        return new ProjectScenario
        {
            Coordinator = coordinator,
            Project = project,
            BankAccount = bankAccount,
            AccountingAccounts = new List<AccountingAccount> { account },
            Transactions = transactions
        };
    }
}

/// <summary>
/// Represents a complete project test scenario with all related entities.
/// </summary>
public class ProjectScenario
{
    public Person Coordinator { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public BankAccount BankAccount { get; set; } = null!;
    public List<AccountingAccount> AccountingAccounts { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
}
