using FluentAssertions;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace ProjectBudgetManagement.IntegrationTests.Api;

public class ProjectsControllerTests : IntegrationTestBase
{
    public ProjectsControllerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task CreateProject_WithValidData_ReturnsCreatedProject()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.SaveChangesAsync();

        var request = new CreateProjectRequest
        {
            Name = "New Project",
            Description = "Test project description",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddMonths(6),
            BudgetAmount = 50000m,
            CoordinatorId = coordinator.Id,
            BankAccount = new BankAccountRequest
            {
                AccountNumber = "98765432",
                BankName = "Test Bank",
                BranchNumber = "0001",
                AccountHolderName = coordinator.Name
            }
        };

        // Act
        var stopwatch = Stopwatch.StartNew();
        var response = await PostAsync("/api/projects", request);
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, "API should respond in less than 100ms");

        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>(JsonOptions);
        project.Should().NotBeNull();
        project!.Name.Should().Be(request.Name);
        project.BudgetAmount.Should().Be(request.BudgetAmount);
        project.Status.Should().Be("NotStarted");
        project.BankAccount.Should().NotBeNull();
        project.BankAccount!.AccountNumber.Should().Be(request.BankAccount.AccountNumber);
    }

    [Fact]
    public async Task GetProject_WithExistingId_ReturnsProject()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        var project = TestDataBuilder.CreateProject(coordinator);
        var bankAccount = TestDataBuilder.CreateBankAccount(project);

        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddAsync(project);
        await DbContext.BankAccounts.AddAsync(bankAccount);
        await DbContext.SaveChangesAsync();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await GetAsync<ProjectResponse>($"/api/projects/{project.Id}");
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(project.Id);
        result.Name.Should().Be(project.Name);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
    }

    [Fact]
    public async Task GetProject_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/projects/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ListProjects_ReturnsAllProjects()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        var project1 = TestDataBuilder.CreateProject(coordinator, "Project 1");
        var project2 = TestDataBuilder.CreateProject(coordinator, "Project 2");
        var bankAccount1 = TestDataBuilder.CreateBankAccount(project1, "11111111");
        var bankAccount2 = TestDataBuilder.CreateBankAccount(project2, "22222222");

        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddRangeAsync(project1, project2);
        await DbContext.BankAccounts.AddRangeAsync(bankAccount1, bankAccount2);
        await DbContext.SaveChangesAsync();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await GetAsync<List<ProjectResponse>>("/api/projects");
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterThanOrEqualTo(2);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
    }

    [Fact]
    public async Task UpdateProject_WithValidData_ReturnsUpdatedProject()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreatePerson();
        var project = TestDataBuilder.CreateProject(coordinator);
        var bankAccount = TestDataBuilder.CreateBankAccount(project);

        await DbContext.Persons.AddAsync(coordinator);
        await DbContext.Projects.AddAsync(project);
        await DbContext.BankAccounts.AddAsync(bankAccount);
        await DbContext.SaveChangesAsync();

        var updateRequest = new UpdateProjectRequest
        {
            Name = "Updated Project Name",
            Description = "Updated description",
            StartDate = project.StartDate,
            EndDate = project.EndDate.AddMonths(1),
            BudgetAmount = 75000m
        };

        // Act
        var response = await PutAsync($"/api/projects/{project.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<ProjectResponse>(JsonOptions);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be(updateRequest.Name);
        updated.BudgetAmount.Should().Be(updateRequest.BudgetAmount);
    }

    [Fact]
    public async Task CreateProject_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            Name = "",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(-1), // End date before start date
            BudgetAmount = -1000m, // Negative budget
            CoordinatorId = Guid.NewGuid()
        };

        // Act
        var response = await PostAsync("/api/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
