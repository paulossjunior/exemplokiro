---
inclusion: manual
---

# Back-End Testing Standards

## Test-Driven Development (TDD)

Always follow the TDD cycle:

1. **Red**: Write a failing test first
2. **Green**: Write minimal code to make the test pass
3. **Refactor**: Improve code while keeping tests green

Benefits:
- Tests drive design and ensure testability
- Better code coverage from the start
- Confidence when refactoring
- Living documentation of expected behavior

## Testing Requirements

All tests must run inside Docker containers to ensure consistency with production environment.

### Database Testing Requirements

- **Tests must use a real SQL Server database (not in-memory)**
- Use realistic test data that mirrors production scenarios
- Seed database with representative data before tests
- Test with actual database constraints, indexes, and relationships
- Verify performance with realistic data volumes
- Clean up test data after each test run

### Test Data Strategy

- Create test data fixtures with realistic values
- Use data builders/factories for consistent test data
- Include edge cases and boundary conditions
- Test with various data volumes to validate performance
- Maintain test data scripts in version control

### Test Database Setup

- Spin up SQL Server container for testing
- Apply migrations to test database
- Seed with realistic test data
- Run tests against real database
- Tear down after test completion

## Running Tests

```bash
# Run all tests in Docker (using Makefile)
make test

# Run specific test project (using Makefile)
make test-filter FILTER="FullyQualifiedName~YourTest"

# Direct Docker commands (if not using Makefile)
docker-compose -f docker-compose.test.yml up --build --abort-on-container-exit
docker-compose -f docker-compose.test.yml run --rm tests dotnet test --filter "FullyQualifiedName~YourTest"
```

## Integration Test Requirements

- Use real database connections
- Test complete workflows end-to-end
- Verify data persistence and retrieval
- Test transaction handling and rollbacks
- Validate database constraints and triggers

## Test Types

### Unit Tests

- Test individual methods and classes in isolation
- Mock external dependencies
- Fast execution (< 100ms per test)
- Focus on business logic and domain rules

### Integration Tests

- Test interaction between components
- Use real database and infrastructure
- Test repository operations
- Verify data persistence and retrieval

### API Tests

- Test complete HTTP request/response cycle
- Verify status codes and response formats
- Test authentication and authorization
- Validate error handling
- Measure response times (< 100ms requirement)
