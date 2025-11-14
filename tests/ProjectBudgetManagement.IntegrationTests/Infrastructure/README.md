# Test Infrastructure Documentation

## Overview

This directory contains the core infrastructure for integration testing the Project Budget Management system. All tests use real SQL Server databases via Testcontainers to ensure accurate testing of database operations, constraints, and performance.

## Key Components

### DatabaseFixture

**Purpose**: Manages a shared SQL Server container for all tests.

**Features**:
- Creates a single SQL Server 2022 container that is shared across all test classes
- Automatically starts the container before tests run
- Creates the database schema using Entity Framework
- Disposes the container after all tests complete

**Usage**:
```csharp
[Collection("Database collection")]
public class MyTests : IntegrationTestBase
{
    public MyTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }
}
```

### IntegrationTestBase

**Purpose**: Base class for all integration tests providing common functionality.

**Features**:
- HTTP client configured with test authentication
- Direct database access via DbContext
- Automatic cleanup after each test
- Helper methods for HTTP operations (GET, POST, PUT, DELETE)
- Performance measurement utilities
- Data seeding helpers

**Key Methods**:
- `GetAsync<T>(url)` - GET request with deserialization
- `PostAsync<T>(url, data)` - POST request
- `PutAsync<T>(url, data)` - PUT request
- `DeleteAsync(url)` - DELETE request
- `SeedAsync<T>(entity)` - Seed single entity to database
- `SeedAsync(params entities)` - Seed multiple entities
- `MeasureAsync<T>(operation)` - Measure operation execution time
- `AssertStatusCode(response, expected)` - Verify HTTP status with detailed error

**Cleanup Strategy**:
Each test automatically cleans up data in the following order:
1. Transactions
2. AuditEntries
3. BankAccounts
4. Projects
5. AccountingAccounts
6. Persons

### IntegrationTestWebAppFactory

**Purpose**: Creates a test instance of the API with test database configuration.

**Features**:
- Replaces production DbContext with test database connection
- Maintains all other service registrations
- Enables testing of the full API stack

### TestConfiguration

**Purpose**: Centralized configuration constants for tests.

**Constants**:
- `MaxResponseTimeMs` - 100ms (API requirement)
- `MaxIntegrationTestResponseTimeMs` - 500ms (more lenient for tests)
- `TestUserId` - Standard test user GUID
- `TestAuthToken` - Default authorization header
- `TestDatabasePassword` - SQL Server password

## Test Data Builders

### TestDataBuilder

**Purpose**: Factory methods for creating test entities with realistic data.

**Key Features**:
- Unique entity generation to avoid conflicts
- Standard test data sets
- Complete scenario builders
- Thread-safe counters for unique values

**Common Methods**:
```csharp
// Create unique entities
var person = TestDataBuilder.CreateUniquePerson();
var project = TestDataBuilder.CreateUniqueProject(coordinator);
var bankAccount = TestDataBuilder.CreateUniqueBankAccount(project);
var account = TestDataBuilder.CreateUniqueAccountingAccount();

// Create complete scenarios
var (coordinator, project, bankAccount, accounts) = 
    TestDataBuilder.CreateCompleteProjectSetup();

// Create standard accounting accounts
var accounts = TestDataBuilder.CreateStandardAccountingAccounts();

// Create transaction sets for balance testing
var transactions = TestDataBuilder.CreateTransactionSet(bankAccount, account);
```

### TestDataSeeder

**Purpose**: Seeds complex, realistic test scenarios into the database.

**Key Methods**:
```csharp
var seeder = new TestDataSeeder(DbContext);

// Seed complete project with transactions
var scenario = await seeder.SeedCompleteProjectScenario();

// Seed multiple projects
var scenarios = await seeder.SeedMultipleProjects(count: 5);

// Seed project with many transactions (performance testing)
var scenario = await seeder.SeedProjectWithManyTransactions(50);

// Seed project at budget limit
var scenario = await seeder.SeedProjectAtBudgetLimit();

// Seed project over budget
var scenario = await seeder.SeedProjectOverBudget();

// Seed audit entries
var entries = await seeder.SeedAuditEntries(entityId, "Project", 10);
```

## Writing Tests

### Basic Test Structure

```csharp
[Collection("Database collection")]
public class MyControllerTests : IntegrationTestBase
{
    public MyControllerTests(DatabaseFixture databaseFixture) 
        : base(databaseFixture)
    {
    }

    [Fact]
    public async Task TestName_Scenario_ExpectedBehavior()
    {
        // Arrange
        var coordinator = TestDataBuilder.CreateUniquePerson();
        await SeedAsync(coordinator);

        // Act
        var (response, elapsed) = await MeasureAsync(async () => 
            await GetAsync<ProjectResponse>($"/api/projects/{project.Id}"));

        // Assert
        response.Should().NotBeNull();
        elapsed.Should().BeLessThan(TestConfiguration.MaxResponseTimeMs);
    }
}
```

### Using Test Data Seeder

```csharp
[Fact]
public async Task ComplexScenario_Test()
{
    // Arrange
    var seeder = new TestDataSeeder(DbContext);
    var scenario = await seeder.SeedCompleteProjectScenario();

    // Act
    var response = await GetAsync<ProjectResponse>(
        $"/api/projects/{scenario.Project.Id}");

    // Assert
    response.Should().NotBeNull();
    response!.BankAccount.Should().NotBeNull();
}
```

### Performance Testing

```csharp
[Fact]
public async Task Endpoint_MeetsPerformanceRequirement()
{
    // Arrange
    var coordinator = TestDataBuilder.CreateUniquePerson();
    await SeedAsync(coordinator);

    // Act
    var elapsed = await MeasureAsync(async () => 
        await Client.GetAsync("/api/projects"));

    // Assert
    elapsed.Should().BeLessThan(
        TestConfiguration.MaxResponseTimeMs,
        "API must respond in less than 100ms");
}
```

## Running Tests

### Prerequisites
- Docker must be running
- .NET 8 SDK installed

### Commands

```bash
# Run all integration tests
dotnet test tests/ProjectBudgetManagement.IntegrationTests

# Run specific test class
dotnet test --filter "FullyQualifiedName~ProjectsControllerTests"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Using Docker Compose (Optional)

If you prefer to manage the test database manually:

```bash
# Start test database
docker-compose -f tests/docker-compose.test.yml up -d

# Run tests against manual database
# (Update connection string in tests if needed)
dotnet test

# Stop test database
docker-compose -f tests/docker-compose.test.yml down -v
```

## Best Practices

### 1. Data Isolation
- Each test should create its own data using unique builders
- Rely on automatic cleanup in `DisposeAsync`
- Don't assume data from other tests exists

### 2. Performance Testing
- Always measure response times for API endpoints
- Use `TestConfiguration.MaxResponseTimeMs` for assertions
- Be aware that first test may be slower due to container startup

### 3. Realistic Data
- Use `TestDataSeeder` for complex scenarios
- Use `TestDataBuilder.CreateUnique*` methods to avoid conflicts
- Create data that reflects real-world usage

### 4. Error Testing
- Test both success and failure scenarios
- Verify correct HTTP status codes
- Check error response format

### 5. Database Constraints
- Test unique constraints
- Test foreign key relationships
- Test check constraints
- Verify constraint violations return appropriate errors

## Troubleshooting

### Tests Fail with "Container not started"
- Ensure Docker is running
- Check Docker has sufficient resources
- Wait for container health check to pass

### Tests Fail with Data Conflicts
- Ensure using `CreateUnique*` methods
- Check cleanup is working properly
- Verify tests are running sequentially (xunit.runner.json)

### Slow Test Execution
- First test is slower due to container startup
- Subsequent tests share the container
- Consider reducing transaction count in performance tests

### Connection Errors
- Verify SQL Server container is healthy
- Check connection string in DatabaseFixture
- Ensure port 1433 is not already in use

## Architecture Decisions

### Why Real Database?
- Tests actual database constraints and behavior
- Validates performance with realistic data
- Catches issues that in-memory databases miss
- Ensures production-like test environment

### Why Shared Container?
- Faster test execution (no container per test)
- Reduced resource usage
- Realistic for CI/CD environments
- Cleanup strategy ensures test isolation

### Why Sequential Execution?
- Prevents data conflicts between tests
- Simplifies debugging
- Ensures predictable test behavior
- Acceptable performance for test suite size
