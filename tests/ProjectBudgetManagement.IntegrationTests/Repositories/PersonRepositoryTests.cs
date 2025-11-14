using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Domain.Entities;
using ProjectBudgetManagement.Infrastructure.Repositories;
using ProjectBudgetManagement.IntegrationTests.Builders;
using ProjectBudgetManagement.IntegrationTests.Infrastructure;

namespace ProjectBudgetManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for PersonRepository with real SQL Server database.
/// </summary>
public class PersonRepositoryTests : IntegrationTestBase
{
    private readonly IPersonRepository _repository;

    public PersonRepositoryTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _repository = new PersonRepository(DbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistPersonToDatabase()
    {
        // Arrange
        var person = TestDataBuilder.CreateUniquePerson("John Doe");

        // Act
        await _repository.AddAsync(person);
        await _repository.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetByIdAsync(person.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(person.Name, retrieved.Name);
        Assert.Equal(person.IdentificationNumber, retrieved.IdentificationNumber);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectPerson()
    {
        // Arrange
        var person = TestDataBuilder.CreateUniquePerson();
        await SeedAsync(person);

        // Act
        var retrieved = await _repository.GetByIdAsync(person.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(person.Id, retrieved.Id);
        Assert.Equal(person.Name, retrieved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Act
        var retrieved = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task GetByIdentificationNumberAsync_ShouldReturnCorrectPerson()
    {
        // Arrange
        var person = TestDataBuilder.CreateUniquePerson();
        await SeedAsync(person);

        // Act
        var retrieved = await _repository.GetByIdentificationNumberAsync(person.IdentificationNumber);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(person.Id, retrieved.Id);
        Assert.Equal(person.IdentificationNumber, retrieved.IdentificationNumber);
    }

    [Fact]
    public async Task GetByIdentificationNumberAsync_WithNonExistentNumber_ShouldReturnNull()
    {
        // Act
        var retrieved = await _repository.GetByIdentificationNumberAsync("99999999999");

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPersons()
    {
        // Arrange
        var person1 = TestDataBuilder.CreateUniquePerson("Alice");
        var person2 = TestDataBuilder.CreateUniquePerson("Bob");
        await SeedAsync(person1, person2);

        // Act
        var persons = await _repository.GetAllAsync();

        // Assert
        Assert.True(persons.Count >= 2);
        Assert.Contains(persons, p => p.Id == person1.Id);
        Assert.Contains(persons, p => p.Id == person2.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPersonsOrderedByName()
    {
        // Arrange
        var personZ = TestDataBuilder.CreateUniquePerson("Zoe");
        var personA = TestDataBuilder.CreateUniquePerson("Alice");
        var personM = TestDataBuilder.CreateUniquePerson("Mike");
        await SeedAsync(personZ, personA, personM);

        // Act
        var persons = await _repository.GetAllAsync();

        // Assert
        var testPersons = persons.Where(p => 
            p.Id == personZ.Id || p.Id == personA.Id || p.Id == personM.Id).ToList();
        
        Assert.Equal(3, testPersons.Count);
        Assert.Equal(personA.Id, testPersons[0].Id); // Alice
        Assert.Equal(personM.Id, testPersons[1].Id); // Mike
        Assert.Equal(personZ.Id, testPersons[2].Id); // Zoe
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnNumberOfAffectedEntities()
    {
        // Arrange
        var person = TestDataBuilder.CreateUniquePerson();
        await _repository.AddAsync(person);

        // Act
        var affectedRows = await _repository.SaveChangesAsync();

        // Assert
        Assert.Equal(1, affectedRows);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateIdentificationNumber_ShouldThrowException()
    {
        // Arrange
        var identificationNumber = "12345678900";
        var person1 = TestDataBuilder.CreatePerson("Person 1", identificationNumber);
        var person2 = TestDataBuilder.CreatePerson("Person 2", identificationNumber);
        
        await _repository.AddAsync(person1);
        await _repository.SaveChangesAsync();

        // Act & Assert
        await _repository.AddAsync(person2);
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await _repository.SaveChangesAsync());
    }
}
