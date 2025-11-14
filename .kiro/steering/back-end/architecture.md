---
inclusion: manual
---

# Back-End Architecture

## Hexagonal Architecture (Ports and Adapters)

The system follows hexagonal architecture principles with clear separation of concerns.

### Core Layers

- **Domain/Core**: Business logic and entities (no external dependencies)
  - Domain entities and value objects
  - Business rules and domain services
  - Domain events
- **Application**: Use cases and application services
  - Application services orchestrating domain logic
  - Commands and queries (CQRS pattern)
  - Input ports (interfaces for use cases)
- **Ports**: Interfaces defining how the core interacts with the outside world
  - **Input Ports**: Interfaces for application services and use cases
  - **Output Ports**: Repository interfaces, external service interfaces
- **Infrastructure**: Implementations of output ports
  - Database repositories (Entity Framework)
  - External service clients
  - File system access
- **API/Presentation**: Input adapters
  - REST controllers
  - Request/response models (DTOs)
  - API validation

### Key Principles

- Domain logic is independent of frameworks and external concerns
- Dependencies point inward (outer layers depend on inner layers)
- Business rules can be tested without UI, database, or external services
- Easy to swap implementations (e.g., switch databases or APIs)
- Use dependency injection to wire up ports and adapters

### Dependency Flow

```
External → API (Input Adapter) → Application (Input Port) → Domain ← Infrastructure (Output Adapter) ← Output Port
```

### Project Structure

```
src/
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Services/
│   └── Events/
├── Application/
│   ├── Commands/
│   ├── Queries/
│   ├── Services/
│   └── Ports/
├── Infrastructure/
│   ├── Persistence/
│   ├── Repositories/
│   └── ExternalServices/
└── Api/
    ├── Controllers/
    ├── Models/
    └── Middleware/
```

## Dependency Injection

**Mandatory use of Dependency Injection throughout the application:**

- Use constructor injection for all dependencies
- Never use service locator pattern or static dependencies
- Register all services in the DI container (Program.cs)
- Use interfaces for all dependencies (follow Dependency Inversion Principle)
- Configure service lifetimes appropriately:
  - **Transient**: Created each time they're requested
  - **Scoped**: Created once per request
  - **Singleton**: Created once for the application lifetime

### DI Best Practices

- Inject interfaces, not concrete implementations
- Keep constructors simple - only assign dependencies
- Avoid injecting too many dependencies (max 3-4, otherwise refactor)
- Use options pattern for configuration injection
- Register services by layer (Domain, Application, Infrastructure)
- Use extension methods to organize service registration

### Example Registration

```csharp
// Program.cs or Startup.cs
services.AddScoped<IExpenseRepository, ExpenseRepository>();
services.AddScoped<IExpenseService, ExpenseService>();
services.AddTransient<IValidator<RegisterExpenseCommand>, RegisterExpenseValidator>();
```

### Example Usage

```csharp
public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repository;
    private readonly IValidator<Expense> _validator;
    
    public ExpenseService(
        IExpenseRepository repository,
        IValidator<Expense> validator)
    {
        _repository = repository;
        _validator = validator;
    }
}
```
