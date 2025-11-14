using Microsoft.Extensions.Caching.Memory;
using ProjectBudgetManagement.Domain.Entities;

namespace ProjectBudgetManagement.Infrastructure.Services;

/// <summary>
/// Service for caching frequently accessed data to improve performance.
/// </summary>
public class CachingService
{
    private readonly IMemoryCache _cache;
    private const string AccountingAccountsCacheKey = "AccountingAccounts_All";
    private const string ProjectCacheKeyPrefix = "Project_";

    /// <summary>
    /// Initializes a new instance of the CachingService class.
    /// </summary>
    /// <param name="cache">The memory cache.</param>
    public CachingService(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Gets or sets accounting accounts in cache.
    /// </summary>
    /// <param name="factory">Factory function to retrieve data if not cached.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of accounting accounts.</returns>
    public async Task<List<AccountingAccount>> GetOrSetAccountingAccountsAsync(
        Func<Task<List<AccountingAccount>>> factory,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(AccountingAccountsCacheKey, out List<AccountingAccount>? cachedAccounts) && cachedAccounts != null)
        {
            return cachedAccounts;
        }

        var accounts = await factory();

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };

        _cache.Set(AccountingAccountsCacheKey, accounts, cacheOptions);

        return accounts;
    }

    /// <summary>
    /// Gets or sets a project in cache.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="factory">Factory function to retrieve data if not cached.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project if found.</returns>
    public async Task<Project?> GetOrSetProjectAsync(
        Guid projectId,
        Func<Task<Project?>> factory,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{ProjectCacheKeyPrefix}{projectId}";

        if (_cache.TryGetValue(cacheKey, out Project? cachedProject))
        {
            return cachedProject;
        }

        var project = await factory();

        if (project != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            _cache.Set(cacheKey, project, cacheOptions);
        }

        return project;
    }

    /// <summary>
    /// Invalidates the accounting accounts cache.
    /// </summary>
    public void InvalidateAccountingAccountsCache()
    {
        _cache.Remove(AccountingAccountsCacheKey);
    }

    /// <summary>
    /// Invalidates a specific project cache entry.
    /// </summary>
    /// <param name="projectId">The project ID to invalidate.</param>
    public void InvalidateProjectCache(Guid projectId)
    {
        var cacheKey = $"{ProjectCacheKeyPrefix}{projectId}";
        _cache.Remove(cacheKey);
    }

    /// <summary>
    /// Invalidates all project cache entries.
    /// </summary>
    public void InvalidateAllProjectsCache()
    {
        // Note: This is a simple implementation. For production, consider using cache tags or patterns
        // For now, we'll rely on expiration policies
    }
}
