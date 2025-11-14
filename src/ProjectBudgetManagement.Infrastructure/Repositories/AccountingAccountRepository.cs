using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Infrastructure.Persistence;
using ProjectBudgetManagement.Infrastructure.Services;

namespace ProjectBudgetManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for AccountingAccount entity.
/// </summary>
public class AccountingAccountRepository : IAccountingAccountRepository
{
    private readonly ProjectBudgetDbContext _context;
    private readonly CachingService? _cachingService;

    // Compiled query for getting accounting account by ID
    private static readonly Func<ProjectBudgetDbContext, Guid, Task<AccountingAccount?>> GetAccountByIdCompiled =
        EF.CompileAsyncQuery((ProjectBudgetDbContext context, Guid id) =>
            context.AccountingAccounts.FirstOrDefault(a => a.Id == id));

    // Compiled query for getting accounting account by identifier
    private static readonly Func<ProjectBudgetDbContext, string, Task<AccountingAccount?>> GetAccountByIdentifierCompiled =
        EF.CompileAsyncQuery((ProjectBudgetDbContext context, string identifier) =>
            context.AccountingAccounts.FirstOrDefault(a => a.Identifier == identifier));

    /// <summary>
    /// Initializes a new instance of the AccountingAccountRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="cachingService">Optional caching service for performance optimization.</param>
    public AccountingAccountRepository(ProjectBudgetDbContext context, CachingService? cachingService = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cachingService = cachingService;
    }

    /// <inheritdoc />
    public async Task<AccountingAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAccountByIdCompiled(_context, id);
    }

    /// <inheritdoc />
    public async Task<AccountingAccount?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await GetAccountByIdentifierCompiled(_context, identifier);
    }

    /// <inheritdoc />
    public async Task<List<AccountingAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Use caching if available for better performance
        if (_cachingService != null)
        {
            return await _cachingService.GetOrSetAccountingAccountsAsync(
                async () => await _context.AccountingAccounts
                    .AsNoTracking()
                    .OrderBy(a => a.Identifier)
                    .ToListAsync(cancellationToken),
                cancellationToken);
        }

        return await _context.AccountingAccounts
            .AsNoTracking()
            .OrderBy(a => a.Identifier)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(AccountingAccount accountingAccount, CancellationToken cancellationToken = default)
    {
        await _context.AccountingAccounts.AddAsync(accountingAccount, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _context.SaveChangesAsync(cancellationToken);
        
        // Invalidate cache after changes
        _cachingService?.InvalidateAccountingAccountsCache();
        
        return result;
    }
}
