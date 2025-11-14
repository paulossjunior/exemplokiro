using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Infrastructure.Repositories;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;

namespace ProjectBudgetManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for AuditRepository with real SQL Server database.
/// </summary>
public class AuditRepositoryTests : IntegrationTestBase
{
    private readonly IAuditRepository _repository;

    public AuditRepositoryTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _repository = new AuditRepository(DbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistAuditEntryToDatabase()
    {
        // Arrange
        var auditEntry = CreateAuditEntry("Project", Guid.NewGuid(), "Create");

        // Act
        await _repository.AddAsync(auditEntry);
        await _repository.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetAuditTrailAsync(entityId: auditEntry.EntityId);
        Assert.NotEmpty(retrieved);
        Assert.Contains(retrieved, a => a.Id == auditEntry.Id);
    }

    [Fact]
    public async Task GetAuditTrailAsync_ShouldReturnAllEntries()
    {
        // Arrange
        var entry1 = CreateAuditEntry("Project", Guid.NewGuid(), "Create");
        var entry2 = CreateAuditEntry("Transaction", Guid.NewGuid(), "Create");
        await SeedAsync(entry1, entry2);

        // Act
        var entries = await _repository.GetAuditTrailAsync();

        // Assert
        Assert.True(entries.Count >= 2);
        Assert.Contains(entries, e => e.Id == entry1.Id);
        Assert.Contains(entries, e => e.Id == entry2.Id);
    }

    [Fact]
    public async Task GetAuditTrailAsync_WithEntityIdFilter_ShouldReturnFilteredEntries()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var entry1 = CreateAuditEntry("Project", entityId, "Create");
        var entry2 = CreateAuditEntry("Project", entityId, "Update");
        var entry3 = CreateAuditEntry("Project", Guid.NewGuid(), "Create");
        await SeedAsync(entry1, entry2, entry3);

        // Act
        var entries = await _repository.GetAuditTrailAsync(entityId: entityId);

        // Assert
        Assert.All(entries, e => Assert.Equal(entityId, e.EntityId));
        Assert.Contains(entries, e => e.Id == entry1.Id);
        Assert.Contains(entries, e => e.Id == entry2.Id);
        Assert.DoesNotContain(entries, e => e.Id == entry3.Id);
    }

    [Fact]
    public async Task GetAuditTrailAsync_WithEntityTypeFilter_ShouldReturnFilteredEntries()
    {
        // Arrange
        var entry1 = CreateAuditEntry("Project", Guid.NewGuid(), "Create");
        var entry2 = CreateAuditEntry("Project", Guid.NewGuid(), "Update");
        var entry3 = CreateAuditEntry("Transaction", Guid.NewGuid(), "Create");
        await SeedAsync(entry1, entry2, entry3);

        // Act
        var entries = await _repository.GetAuditTrailAsync(entityType: "Project");

        // Assert
        Assert.All(entries, e => Assert.Equal("Project", e.EntityType));
        Assert.Contains(entries, e => e.Id == entry1.Id);
        Assert.Contains(entries, e => e.Id == entry2.Id);
        Assert.DoesNotContain(entries, e => e.Id == entry3.Id);
    }

    [Fact]
    public async Task GetAuditTrailAsync_WithUserIdFilter_ShouldReturnFilteredEntries()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var entry1 = CreateAuditEntry("Project", Guid.NewGuid(), "Create", userId);
        var entry2 = CreateAuditEntry("Transaction", Guid.NewGuid(), "Create", userId);
        var entry3 = CreateAuditEntry("Project", Guid.NewGuid(), "Create", Guid.NewGuid());
        await SeedAsync(entry1, entry2, entry3);

        // Act
        var entries = await _repository.GetAuditTrailAsync(userId: userId);

        // Assert
        Assert.All(entries, e => Assert.Equal(userId, e.UserId));
        Assert.Contains(entries, e => e.Id == entry1.Id);
        Assert.Contains(entries, e => e.Id == entry2.Id);
        Assert.DoesNotContain(entries, e => e.Id == entry3.Id);
    }

    [Fact]
    public async Task GetAuditTrailAsync_WithDateRangeFilter_ShouldReturnFilteredEntries()
    {
        // Arrange
        var oldDate = DateTime.UtcNow.AddDays(-30);
        var recentDate = DateTime.UtcNow.AddDays(-5);
        var todayDate = DateTime.UtcNow;
        
        var oldEntry = CreateAuditEntry("Project", Guid.NewGuid(), "Create", timestamp: oldDate);
        var recentEntry = CreateAuditEntry("Project", Guid.NewGuid(), "Update", timestamp: recentDate);
        var todayEntry = CreateAuditEntry("Project", Guid.NewGuid(), "Update", timestamp: todayDate);
        await SeedAsync(oldEntry, recentEntry, todayEntry);

        // Act
        var entries = await _repository.GetAuditTrailAsync(
            startDate: DateTime.UtcNow.AddDays(-10),
            endDate: DateTime.UtcNow);

        // Assert
        Assert.Contains(entries, e => e.Id == recentEntry.Id);
        Assert.Contains(entries, e => e.Id == todayEntry.Id);
        Assert.DoesNotContain(entries, e => e.Id == oldEntry.Id);
    }

    [Fact]
    public async Task GetAuditTrailAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var entries = new List<AuditEntry>();
        for (int i = 0; i < 5; i++)
        {
            entries.Add(CreateAuditEntry("Project", Guid.NewGuid(), "Create"));
        }
        foreach (var entry in entries)
        {
            await SeedAsync(entry);
        }

        // Act
        var firstPage = await _repository.GetAuditTrailAsync(skip: 0, take: 2);
        var secondPage = await _repository.GetAuditTrailAsync(skip: 2, take: 2);

        // Assert
        Assert.Equal(2, firstPage.Count);
        Assert.Equal(2, secondPage.Count);
        Assert.NotEqual(firstPage[0].Id, secondPage[0].Id);
    }

    [Fact]
    public async Task GetAuditTrailAsync_ShouldReturnEntriesInDescendingTimestampOrder()
    {
        // Arrange
        var date1 = DateTime.UtcNow.AddDays(-10);
        var date2 = DateTime.UtcNow.AddDays(-5);
        var date3 = DateTime.UtcNow;
        
        var entry1 = CreateAuditEntry("Project", Guid.NewGuid(), "Create", timestamp: date1);
        var entry2 = CreateAuditEntry("Project", Guid.NewGuid(), "Update", timestamp: date2);
        var entry3 = CreateAuditEntry("Project", Guid.NewGuid(), "Update", timestamp: date3);
        await SeedAsync(entry1, entry2, entry3);

        // Act
        var entries = await _repository.GetAuditTrailAsync();

        // Assert
        var testEntries = entries.Where(e => 
            e.Id == entry1.Id || e.Id == entry2.Id || e.Id == entry3.Id).ToList();
        
        Assert.Equal(3, testEntries.Count);
        Assert.Equal(entry3.Id, testEntries[0].Id); // Most recent
        Assert.Equal(entry2.Id, testEntries[1].Id);
        Assert.Equal(entry1.Id, testEntries[2].Id); // Oldest
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnNumberOfAffectedEntities()
    {
        // Arrange
        var auditEntry = CreateAuditEntry("Project", Guid.NewGuid(), "Create");
        await _repository.AddAsync(auditEntry);

        // Act
        var affectedRows = await _repository.SaveChangesAsync();

        // Assert
        Assert.Equal(1, affectedRows);
    }

    [Fact]
    public async Task GetAuditTrailAsync_WithMultipleFilters_ShouldReturnCorrectEntries()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var recentDate = DateTime.UtcNow.AddDays(-5);
        
        var matchingEntry = CreateAuditEntry("Project", entityId, "Create", userId, recentDate);
        var wrongEntity = CreateAuditEntry("Project", Guid.NewGuid(), "Create", userId, recentDate);
        var wrongUser = CreateAuditEntry("Project", entityId, "Create", Guid.NewGuid(), recentDate);
        var wrongDate = CreateAuditEntry("Project", entityId, "Create", userId, DateTime.UtcNow.AddDays(-30));
        
        await SeedAsync(matchingEntry, wrongEntity, wrongUser, wrongDate);

        // Act
        var entries = await _repository.GetAuditTrailAsync(
            entityId: entityId,
            userId: userId,
            startDate: DateTime.UtcNow.AddDays(-10));

        // Assert
        Assert.Contains(entries, e => e.Id == matchingEntry.Id);
        Assert.DoesNotContain(entries, e => e.Id == wrongEntity.Id);
        Assert.DoesNotContain(entries, e => e.Id == wrongUser.Id);
        Assert.DoesNotContain(entries, e => e.Id == wrongDate.Id);
    }

    private AuditEntry CreateAuditEntry(
        string entityType,
        Guid entityId,
        string actionType,
        Guid? userId = null,
        DateTime? timestamp = null)
    {
        return new AuditEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? TestUserId,
            ActionType = actionType,
            EntityType = entityType,
            EntityId = entityId,
            Timestamp = timestamp ?? DateTime.UtcNow,
            PreviousValue = null,
            NewValue = "{}",
            DigitalSignature = $"SIGNATURE_{Guid.NewGuid()}",
            DataHash = $"HASH_{Guid.NewGuid()}"
        };
    }
}
