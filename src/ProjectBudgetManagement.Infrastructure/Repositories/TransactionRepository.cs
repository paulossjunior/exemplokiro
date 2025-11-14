using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.Infrastructure.Persistence;

namespace ProjectBudgetManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Transaction entity.
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly ProjectBudgetDbContext _context;

    // Compiled query for getting transaction by ID with related entities
    private static readonly Func<ProjectBudgetDbContext, Guid, Task<Transaction?>> GetTransactionByIdCompiled =
        EF.CompileAsyncQuery((ProjectBudgetDbContext context, Guid id) =>
            context.Transactions
                .Include(t => t.BankAccount)
                .Include(t => t.AccountingAccount)
                .FirstOrDefault(t => t.Id == id));

    /// <summary>
    /// Initializes a new instance of the TransactionRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TransactionRepository(ProjectBudgetDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetTransactionByIdCompiled(_context, id);
    }

    /// <inheritdoc />
    public async Task<List<Transaction>> GetByBankAccountAsync(
        Guid bankAccountId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        TransactionClassification? classification = null,
        Guid? accountingAccountId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Transactions
            .Include(t => t.AccountingAccount)
            .Where(t => t.BankAccountId == bankAccountId)
            .AsNoTracking();

        if (startDate.HasValue)
        {
            query = query.Where(t => t.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.Date <= endDate.Value);
        }

        if (classification.HasValue)
        {
            query = query.Where(t => t.Classification == classification.Value);
        }

        if (accountingAccountId.HasValue)
        {
            query = query.Where(t => t.AccountingAccountId == accountingAccountId.Value);
        }

        return await query
            .OrderBy(t => t.Date)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(transaction, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
