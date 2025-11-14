using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;

namespace ProjectBudgetManagement.Infrastructure.Persistence;

/// <summary>
/// Database context for the Project Budget Management System.
/// </summary>
public class ProjectBudgetDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the ProjectBudgetDbContext class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public ProjectBudgetDbContext(DbContextOptions<ProjectBudgetDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Projects DbSet.
    /// </summary>
    public DbSet<Project> Projects { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Persons DbSet.
    /// </summary>
    public DbSet<Person> Persons { get; set; } = null!;

    /// <summary>
    /// Gets or sets the BankAccounts DbSet.
    /// </summary>
    public DbSet<BankAccount> BankAccounts { get; set; } = null!;

    /// <summary>
    /// Gets or sets the AccountingAccounts DbSet.
    /// </summary>
    public DbSet<AccountingAccount> AccountingAccounts { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Transactions DbSet.
    /// </summary>
    public DbSet<Transaction> Transactions { get; set; } = null!;

    /// <summary>
    /// Gets or sets the AuditEntries DbSet.
    /// </summary>
    public DbSet<AuditEntry> AuditEntries { get; set; } = null!;

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProject(modelBuilder);
        ConfigurePerson(modelBuilder);
        ConfigureBankAccount(modelBuilder);
        ConfigureAccountingAccount(modelBuilder);
        ConfigureTransaction(modelBuilder);
        ConfigureAuditEntry(modelBuilder);
    }

    private void ConfigureProject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.StartDate)
                .IsRequired();

            entity.Property(e => e.EndDate)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.BudgetAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Relationships
            entity.HasOne(e => e.Coordinator)
                .WithMany(p => p.CoordinatedProjects)
                .HasForeignKey(e => e.CoordinatorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.BankAccount)
                .WithOne(b => b.Project)
                .HasForeignKey<BankAccount>(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraints
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Projects_Dates", "[StartDate] <= [EndDate]");
                t.HasCheckConstraint("CK_Projects_Budget", "[BudgetAmount] > 0");
            });

            // Indexes
            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Projects_Status");
        });
    }

    private void ConfigurePerson(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.IdentificationNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            // Unique constraint
            entity.HasIndex(e => e.IdentificationNumber)
                .IsUnique()
                .HasDatabaseName("UQ_Persons_IdentificationNumber");
        });
    }

    private void ConfigureBankAccount(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AccountNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.BankName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.BranchNumber)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.AccountHolderName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            // Unique constraint for bank account combination
            entity.HasIndex(e => new { e.AccountNumber, e.BankName, e.BranchNumber })
                .IsUnique()
                .HasDatabaseName("UQ_BankAccounts");

            // Unique constraint for ProjectId (one-to-one)
            entity.HasIndex(e => e.ProjectId)
                .IsUnique()
                .HasDatabaseName("UQ_BankAccounts_ProjectId");
        });
    }

    private void ConfigureAccountingAccount(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountingAccount>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Identifier)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            // Unique constraint
            entity.HasIndex(e => e.Identifier)
                .IsUnique()
                .HasDatabaseName("UQ_AccountingAccounts_Identifier");
        });
    }

    private void ConfigureTransaction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Date)
                .IsRequired();

            entity.Property(e => e.Classification)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.DigitalSignature)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.DataHash)
                .IsRequired()
                .HasMaxLength(64);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .IsRequired();

            // Relationships
            entity.HasOne(e => e.BankAccount)
                .WithMany(b => b.Transactions)
                .HasForeignKey(e => e.BankAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AccountingAccount)
                .WithMany(a => a.Transactions)
                .HasForeignKey(e => e.AccountingAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraint
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Transactions_Amount", "[Amount] > 0");
            });

            // Indexes
            entity.HasIndex(e => e.Date)
                .HasDatabaseName("IX_Transactions_Date");

            entity.HasIndex(e => e.BankAccountId)
                .HasDatabaseName("IX_Transactions_BankAccountId");
        });
    }

    private void ConfigureAuditEntry(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId)
                .IsRequired();

            entity.Property(e => e.ActionType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.EntityId)
                .IsRequired();

            entity.Property(e => e.Timestamp)
                .IsRequired();

            entity.Property(e => e.PreviousValue)
                .HasMaxLength(4000);

            entity.Property(e => e.NewValue)
                .HasMaxLength(4000);

            entity.Property(e => e.DigitalSignature)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.DataHash)
                .IsRequired()
                .HasMaxLength(64);

            // Indexes
            entity.HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_AuditEntries_Timestamp");

            entity.HasIndex(e => e.EntityId)
                .HasDatabaseName("IX_AuditEntries_EntityId");
        });
    }
}
