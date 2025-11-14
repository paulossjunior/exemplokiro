using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Infrastructure.Persistence;

namespace ProjectBudgetManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for AccountingAccount entity.
/// </summary>
public class AccountingAccountRepository : IAccountingAccountRepository
{
    private readonly ProjectBudgetDbContext _context;

    /// <summary>
    /// Initializes a new instance of the AccountingAccountRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AccountingAccountRepository(ProjectBudgetDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<AccountingAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AccountingAccounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AccountingAccount?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await _context.AccountingAccounts
            .FirstOrDefaultAsync(a => a.Identifier == identifier, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AccountingAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
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
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
