using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectBudgetManagement.Infrastructure.Persistence;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjectBudgetManagement.IntegrationTests.Infrastructure;

[Collection("Database collection")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly IntegrationTestWebAppFactory Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly ProjectBudgetDbContext DbContext;

    protected readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Test user ID that matches the one returned by GetUserIdFromToken in the controller
    protected static readonly Guid TestUserId = TestConfiguration.TestUserId;

    protected IntegrationTestBase(DatabaseFixture databaseFixture)
    {
        Factory = new IntegrationTestWebAppFactory(databaseFixture.ConnectionString);
        Client = Factory.CreateClient();
        
        // Add default authorization header for tests
        Client.DefaultRequestHeaders.Add("Authorization", TestConfiguration.TestAuthToken);
        
        Scope = Factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<ProjectBudgetDbContext>();
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual async Task DisposeAsync()
    {
        // Clean up test data after each test in reverse dependency order
        try
        {
            // Clear change tracker to avoid conflicts
            DbContext.ChangeTracker.Clear();
            
            // Delete in order of dependencies (child to parent)
            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Transactions]");
            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AuditEntries]");
            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [BankAccounts]");
            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Projects]");
            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AccountingAccounts]");
            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Persons]");
        }
        catch
        {
            // Ignore cleanup errors
        }
        finally
        {
            Scope.Dispose();
            Client.Dispose();
            Factory.Dispose();
        }
    }

    protected async Task<T?> GetAsync<T>(string url)
    {
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
    {
        return await Client.PostAsJsonAsync(url, data);
    }

    protected async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
    {
        return await Client.PutAsJsonAsync(url, data);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await Client.DeleteAsync(url);
    }

    /// <summary>
    /// Seeds the database with test data and returns the entities.
    /// </summary>
    protected async Task<T> SeedAsync<T>(T entity) where T : class
    {
        await DbContext.AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Seeds multiple entities to the database.
    /// </summary>
    protected async Task SeedAsync(params object[] entities)
    {
        foreach (var entity in entities)
        {
            await DbContext.AddAsync(entity);
        }
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Verifies that an HTTP response has the expected status code.
    /// </summary>
    protected void AssertStatusCode(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        if (response.StatusCode != expectedStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            throw new Exception($"Expected status code {expectedStatusCode} but got {response.StatusCode}. Response: {content}");
        }
    }

    /// <summary>
    /// Measures the execution time of an async operation.
    /// </summary>
    protected async Task<(T result, long milliseconds)> MeasureAsync<T>(Func<Task<T>> operation)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await operation();
        stopwatch.Stop();
        return (result, stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Measures the execution time of an async operation without return value.
    /// </summary>
    protected async Task<long> MeasureAsync(Func<Task> operation)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await operation();
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }
}
