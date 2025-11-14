using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectBudgetManagement.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace ProjectBudgetManagement.IntegrationTests.Infrastructure;

/// <summary>
/// Shared database fixture that creates a single SQL Server container for all tests.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Test@1234")
        .Build();

    public string ConnectionString => _dbContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        // Create the database schema using EnsureCreated (for testing without migrations)
        var options = new DbContextOptionsBuilder<ProjectBudgetDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ProjectBudgetDbContext(options);
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}

/// <summary>
/// Collection definition to share the database fixture across all test classes.
/// </summary>
[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}
