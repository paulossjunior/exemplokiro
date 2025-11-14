using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Domain.ValueObjects;
using ProjectBudgetManagement.Infrastructure.Repositories;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;

namespace ProjectBudgetManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for ProjectRepository with real SQL Server database.
/// </summary>
public class ProjectRepositoryTests : IntegrationTestBase
{
    private readonly IProjectRepository _repository;

    public ProjectRepositoryTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _repository = new ProjectRepository(DbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistProjectToDatabase()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        await SeedAsync(coordinator);
        
        var project = TestDataBuilder.CreateUniqueProject(coordinator);

        // Act
        await _repository.AddAsync(project);
        await _repository.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetByIdAsync(project.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(project.Name, retrieved.Name);
        Assert.Equal(project.BudgetAmount, retrieved.BudgetAmount);
        Assert.Equal(project.Status, retrieved.Status);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeCoordinatorAndBankAccount()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project = TestDataBuilder.CreateUniqueProject(coordinator);
        var bankAccount = TestDataBuilder.CreateUniqueBankAccount(project);
        await SeedAsync(coordinator, project, bankAccount);

        // Act
        var retrieved = await _repository.GetByIdAsync(project.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.NotNull(retrieved.Coordinator);
        Assert.Equal(coordinator.Name, retrieved.Coordinator.Name);
        Assert.NotNull(retrieved.BankAccount);
        Assert.Equal(bankAccount.AccountNumber, retrieved.BankAccount.AccountNumber);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProjects()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project1 = TestDataBuilder.CreateUniqueProject(coordinator, ProjectStatus.NotStarted);
        var project2 = TestDataBuilder.CreateUniqueProject(coordinator, ProjectStatus.InProgress);
        await SeedAsync(coordinator, project1, project2);

        // Act
        var projects = await _repository.GetAllAsync();

        // Assert
        Assert.True(projects.Count >= 2);
        Assert.Contains(projects, p => p.Id == project1.Id);
        Assert.Contains(projects, p => p.Id == project2.Id);
    }

    [Fact]
    public async Task GetAllAsync_WithStatusFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var activeProject = TestDataBuilder.CreateUniqueProject(coordinator, ProjectStatus.InProgress);
        var completedProject = TestDataBuilder.CreateUniqueProject(coordinator, ProjectStatus.Completed);
        await SeedAsync(coordinator, activeProject, completedProject);

        // Act
        var inProgressProjects = await _repository.GetAllAsync(status: ProjectStatus.InProgress);

        // Assert
        Assert.All(inProgressProjects, p => Assert.Equal(ProjectStatus.InProgress, p.Status));
        Assert.Contains(inProgressProjects, p => p.Id == activeProject.Id);
        Assert.DoesNotContain(inProgressProjects, p => p.Id == completedProject.Id);
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var projects = new List<Project>();
        for (int i = 0; i < 5; i++)
        {
            projects.Add(TestDataBuilder.CreateUniqueProject(coordinator));
        }
        await SeedAsync(coordinator);
        foreach (var project in projects)
        {
            await SeedAsync(project);
        }

        // Act
        var firstPage = await _repository.GetAllAsync(skip: 0, take: 2);
        var secondPage = await _repository.GetAllAsync(skip: 2, take: 2);

        // Assert
        Assert.Equal(2, firstPage.Count);
        Assert.Equal(2, secondPage.Count);
        Assert.NotEqual(firstPage[0].Id, secondPage[0].Id);
    }

    [Fact]
    public async Task GetCountAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project1 = TestDataBuilder.CreateUniqueProject(coordinator);
        var project2 = TestDataBuilder.CreateUniqueProject(coordinator);
        await SeedAsync(coordinator, project1, project2);

        // Act
        var count = await DbContext.Projects.CountAsync();

        // Assert
        Assert.True(count >= 2);
    }

    [Fact]
    public async Task GetCountAsync_WithStatusFilter_ShouldReturnFilteredCount()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var activeProject1 = TestDataBuilder.CreateUniqueProject(coordinator, ProjectStatus.InProgress);
        var activeProject2 = TestDataBuilder.CreateUniqueProject(coordinator, ProjectStatus.InProgress);
        var completedProject = TestDataBuilder.CreateUniqueProject(coordinator, ProjectStatus.Completed);
        await SeedAsync(coordinator, activeProject1, activeProject2, completedProject);

        // Act
        var inProgressCount = await DbContext.Projects.CountAsync(p => p.Status == ProjectStatus.InProgress);

        // Assert
        Assert.True(inProgressCount >= 2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project = TestDataBuilder.CreateUniqueProject(coordinator);
        await SeedAsync(coordinator, project);

        // Act
        project.Status = ProjectStatus.InProgress;
        project.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(project);
        await _repository.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetByIdAsync(project.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(ProjectStatus.InProgress, retrieved.Status);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnNumberOfAffectedEntities()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        var project = TestDataBuilder.CreateUniqueProject(coordinator);
        await SeedAsync(coordinator);
        await _repository.AddAsync(project);

        // Act
        var affectedRows = await _repository.SaveChangesAsync();

        // Assert
        Assert.Equal(1, affectedRows);
    }
}
