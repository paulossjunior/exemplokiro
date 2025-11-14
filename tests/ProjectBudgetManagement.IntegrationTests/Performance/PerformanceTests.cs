using FluentAssertions;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace ProjectBudgetManagement.IntegrationTests.Performance;

/// <summary>
/// Performance tests to verify all endpoints meet the <100ms response time requirement.
/// Tests with realistic data volumes and concurrent users.
/// </summary>
[Collection("Database collection")]
public class PerformanceTests : IntegrationTestBase
{
    private readonly ITestOutputHelper _output;
    private const int MaxResponseTimeMs = 100;
    private const int WarmupIterations = 3;
    private const int TestIterations = 10;

    public PerformanceTests(DatabaseFixture databaseFixture, ITestOutputHelper output) 
        : base(databaseFixture)
    {
        _output = output;
    }

    #region Project Endpoints Performance

    [Fact]
    public async Task CreateProject_MeetsPerformanceRequirement()
    {
        // Arrange
        var coordinator = await CreateTestCoordinator();
        var request = CreateValidProjectRequest(coordinator.Id);

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await PostAsync("/api/projects", request);
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await PostAsync("/api/projects", request);
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"CreateProject - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs, 
            $"Average response time should be less than {MaxResponseTimeMs}ms");
        maxTime.Should().BeLessThan((long)((long)(MaxResponseTimeMs * 1.5)), 
            $"Maximum response time should be less than {(long)(MaxResponseTimeMs * 1.5)}ms");
    }

    [Fact]
    public async Task GetProject_MeetsPerformanceRequirement()
    {
        // Arrange
        var project = await CreateTestProjectWithBankAccount();

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await Client.GetAsync($"/api/projects/{project.Id}");
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await Client.GetAsync($"/api/projects/{project.Id}");
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"GetProject - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    [Fact]
    public async Task ListProjects_WithMultipleProjects_MeetsPerformanceRequirement()
    {
        // Arrange - Create 50 projects
        var coordinator = await CreateTestCoordinator();
        for (int i = 0; i < 50; i++)
        {
            await CreateTestProjectWithBankAccount($"Project {i}", coordinator);
        }

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await Client.GetAsync("/api/projects");
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await Client.GetAsync("/api/projects");
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"ListProjects (50 items) - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    [Fact]
    public async Task UpdateProject_MeetsPerformanceRequirement()
    {
        // Arrange
        var project = await CreateTestProjectWithBankAccount();
        var updateRequest = new UpdateProjectRequest
        {
            Name = "Updated Project Name",
            Description = "Updated description",
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            BudgetAmount = 150000m
        };

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await PutAsync($"/api/projects/{project.Id}", updateRequest);
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await PutAsync($"/api/projects/{project.Id}", updateRequest);
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"UpdateProject - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    #endregion

    #region Transaction Endpoints Performance

    [Fact]
    public async Task CreateTransaction_MeetsPerformanceRequirement()
    {
        // Arrange
        var project = await CreateTestProjectWithBankAccount();
        var accountingAccount = await CreateTestAccountingAccount();
        var request = new CreateTransactionRequest
        {
            Amount = 1000m,
            Date = DateTime.UtcNow.Date,
            Classification = "Credit",
            AccountingAccountId = accountingAccount.Id
        };

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await PostAsync($"/api/projects/{project.Id}/transactions", request);
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await PostAsync($"/api/projects/{project.Id}/transactions", request);
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"CreateTransaction - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    [Fact]
    public async Task GetTransactionHistory_WithManyTransactions_MeetsPerformanceRequirement()
    {
        // Arrange - Create project with 100 transactions
        var project = await CreateTestProjectWithBankAccount();
        var accountingAccount = await CreateTestAccountingAccount();
        
        for (int i = 0; i < 100; i++)
        {
            await CreateTestTransaction(project.BankAccount, accountingAccount, 1000m + i);
        }

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await Client.GetAsync($"/api/projects/{project.Id}/transactions");
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await Client.GetAsync($"/api/projects/{project.Id}/transactions");
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"GetTransactionHistory (100 items) - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    [Fact]
    public async Task GetAccountBalance_MeetsPerformanceRequirement()
    {
        // Arrange
        var project = await CreateTestProjectWithBankAccount();
        var accountingAccount = await CreateTestAccountingAccount();
        
        // Create some transactions for realistic scenario
        for (int i = 0; i < 20; i++)
        {
            await CreateTestTransaction(project.BankAccount, accountingAccount, 1000m);
        }

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await Client.GetAsync($"/api/projects/{project.Id}/balance");
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await Client.GetAsync($"/api/projects/{project.Id}/balance");
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"GetAccountBalance - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    #endregion

    #region Accounting Account Endpoints Performance

    [Fact]
    public async Task CreateAccountingAccount_MeetsPerformanceRequirement()
    {
        // Arrange
        var request = new CreateAccountingAccountRequest
        {
            Name = "Test Account",
            Identifier = "1000.01.0001"
        };

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            request.Identifier = $"1000.01.{1000 + i:D4}";
            await PostAsync("/api/accounting-accounts", request);
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            request.Identifier = $"2000.01.{1000 + i:D4}";
            
            var stopwatch = Stopwatch.StartNew();
            var response = await PostAsync("/api/accounting-accounts", request);
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"CreateAccountingAccount - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    [Fact]
    public async Task ListAccountingAccounts_WithManyAccounts_MeetsPerformanceRequirement()
    {
        // Arrange - Create 50 accounting accounts
        for (int i = 0; i < 50; i++)
        {
            await CreateTestAccountingAccount($"Account {i}", $"3000.01.{1000 + i:D4}");
        }

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await Client.GetAsync("/api/accounting-accounts");
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await Client.GetAsync("/api/accounting-accounts");
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"ListAccountingAccounts (50 items) - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    [Fact]
    public async Task GetAccountingAccount_MeetsPerformanceRequirement()
    {
        // Arrange
        var account = await CreateTestAccountingAccount();

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await Client.GetAsync($"/api/accounting-accounts/{account.Id}");
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await Client.GetAsync($"/api/accounting-accounts/{account.Id}");
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"GetAccountingAccount - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    #endregion

    #region Audit Endpoints Performance

    [Fact]
    public async Task GetAuditTrail_WithManyEntries_MeetsPerformanceRequirement()
    {
        // Arrange - Create activities that generate audit entries
        var coordinator = await CreateTestCoordinator();
        for (int i = 0; i < 30; i++)
        {
            await CreateTestProjectWithBankAccount($"Project {i}", coordinator);
        }

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await Client.GetAsync("/api/audit/trail");
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await Client.GetAsync("/api/audit/trail");
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"GetAuditTrail (many entries) - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    [Fact]
    public async Task VerifyDataIntegrity_MeetsPerformanceRequirement()
    {
        // Arrange - Create some data
        var project = await CreateTestProjectWithBankAccount();
        var accountingAccount = await CreateTestAccountingAccount();
        for (int i = 0; i < 10; i++)
        {
            await CreateTestTransaction(project.BankAccount, accountingAccount, 1000m);
        }

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await Client.GetAsync("/api/audit/integrity");
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await Client.GetAsync("/api/audit/integrity");
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"VerifyDataIntegrity - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    #endregion

    #region Report Endpoints Performance

    [Fact]
    public async Task GenerateAccountabilityReport_MeetsPerformanceRequirement()
    {
        // Arrange
        var project = await CreateTestProjectWithBankAccount();
        var accountingAccount = await CreateTestAccountingAccount();
        
        // Create transactions for realistic report
        for (int i = 0; i < 20; i++)
        {
            await CreateTestTransaction(project.BankAccount, accountingAccount, 1000m + i);
        }

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            await PostAsync($"/api/reports/accountability/{project.Id}", new { });
        }

        // Act & Assert
        var times = new List<long>();
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await PostAsync($"/api/reports/accountability/{project.Id}", new { });
            stopwatch.Stop();
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"GenerateAccountabilityReport - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 1.5));
    }

    #endregion

    #region Concurrent User Tests

    [Fact]
    public async Task ConcurrentProjectCreation_MeetsPerformanceRequirement()
    {
        // Arrange
        var coordinator = await CreateTestCoordinator();
        const int concurrentUsers = 10;

        // Act
        var tasks = new List<Task<(HttpResponseMessage response, long milliseconds)>>();
        
        for (int i = 0; i < concurrentUsers; i++)
        {
            var request = CreateValidProjectRequest(coordinator.Id, $"Concurrent Project {i}");
            tasks.Add(Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await PostAsync("/api/projects", request);
                stopwatch.Stop();
                return (response, stopwatch.ElapsedMilliseconds);
            }));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        foreach (var (response, _) in results)
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        var times = results.Select(r => r.milliseconds).ToList();
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"ConcurrentProjectCreation ({concurrentUsers} users) - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan((long)(MaxResponseTimeMs * 2), 
            "Average response time under concurrent load should be reasonable");
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 3), 
            "Maximum response time under concurrent load should be reasonable");
    }

    [Fact]
    public async Task ConcurrentTransactionCreation_MeetsPerformanceRequirement()
    {
        // Arrange
        var project = await CreateTestProjectWithBankAccount();
        var accountingAccount = await CreateTestAccountingAccount();
        const int concurrentUsers = 10;

        // Act
        var tasks = new List<Task<(HttpResponseMessage response, long milliseconds)>>();
        
        for (int i = 0; i < concurrentUsers; i++)
        {
            var request = new CreateTransactionRequest
            {
                Amount = 1000m + i,
                Date = DateTime.UtcNow.Date,
                Classification = "Credit",
                AccountingAccountId = accountingAccount.Id
            };
            
            tasks.Add(Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await PostAsync($"/api/projects/{project.Id}/transactions", request);
                stopwatch.Stop();
                return (response, stopwatch.ElapsedMilliseconds);
            }));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        foreach (var (response, _) in results)
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        var times = results.Select(r => r.milliseconds).ToList();
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"ConcurrentTransactionCreation ({concurrentUsers} users) - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan((long)(MaxResponseTimeMs * 2));
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 3));
    }

    [Fact]
    public async Task ConcurrentReadOperations_MeetsPerformanceRequirement()
    {
        // Arrange
        var project = await CreateTestProjectWithBankAccount();
        const int concurrentUsers = 20;

        // Act
        var tasks = new List<Task<(HttpResponseMessage response, long milliseconds)>>();
        
        for (int i = 0; i < concurrentUsers; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await Client.GetAsync($"/api/projects/{project.Id}");
                stopwatch.Stop();
                return (response, stopwatch.ElapsedMilliseconds);
            }));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        foreach (var (response, _) in results)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        var times = results.Select(r => r.milliseconds).ToList();
        var avgTime = times.Average();
        var maxTime = times.Max();
        
        _output.WriteLine($"ConcurrentReadOperations ({concurrentUsers} users) - Avg: {avgTime:F2}ms, Max: {maxTime}ms, Min: {times.Min()}ms");
        
        avgTime.Should().BeLessThan(MaxResponseTimeMs);
        maxTime.Should().BeLessThan((long)(MaxResponseTimeMs * 2));
    }

    #endregion

    #region Helper Methods

    private async Task<Person> CreateTestCoordinator(string name = "Test Coordinator")
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = name,
            IdentificationNumber = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };
        return await SeedAsync(person);
    }

    private async Task<Project> CreateTestProjectWithBankAccount(string name = "Test Project", Person? coordinator = null)
    {
        coordinator ??= await CreateTestCoordinator();

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = "Test Description",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.AddMonths(6).Date,
            Status = ProjectStatus.InProgress,
            BudgetAmount = 100000m,
            CoordinatorId = coordinator.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var bankAccount = new BankAccount
        {
            Id = Guid.NewGuid(),
            AccountNumber = Guid.NewGuid().ToString("N").Substring(0, 10),
            BankName = "Test Bank",
            BranchNumber = "001",
            AccountHolderName = coordinator.Name,
            ProjectId = project.Id,
            CreatedAt = DateTime.UtcNow
        };

        project.BankAccount = bankAccount;

        await SeedAsync(project, bankAccount);
        return project;
    }

    private async Task<AccountingAccount> CreateTestAccountingAccount(string name = "Test Account", string? identifier = null)
    {
        var account = new AccountingAccount
        {
            Id = Guid.NewGuid(),
            Name = name,
            Identifier = identifier ?? $"1000.01.{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            CreatedAt = DateTime.UtcNow
        };
        return await SeedAsync(account);
    }

    private async Task<Transaction> CreateTestTransaction(BankAccount bankAccount, AccountingAccount accountingAccount, decimal amount)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Date = DateTime.UtcNow.Date,
            Classification = TransactionClassification.Credit,
            DigitalSignature = "test-signature",
            DataHash = "test-hash",
            BankAccountId = bankAccount.Id,
            AccountingAccountId = accountingAccount.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = TestUserId
        };
        return await SeedAsync(transaction);
    }

    private CreateProjectRequest CreateValidProjectRequest(Guid coordinatorId, string name = "Test Project")
    {
        return new CreateProjectRequest
        {
            Name = name,
            Description = "Test project description",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.AddMonths(6).Date,
            BudgetAmount = 100000m,
            CoordinatorId = coordinatorId,
            BankAccount = new BankAccountRequest
            {
                AccountNumber = Guid.NewGuid().ToString("N").Substring(0, 10),
                BankName = "Test Bank",
                BranchNumber = "001"
            }
        };
    }

    #endregion
}
