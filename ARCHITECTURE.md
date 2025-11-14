# Architecture Documentation

## Hexagonal Architecture Overview

This project follows the **Hexagonal Architecture** (also known as Ports and Adapters) pattern, which provides clear separation of concerns and makes the system highly testable and maintainable.

## Layer Structure

### 1. Domain Layer (`ProjectBudgetManagement.Domain`)

**Purpose**: Contains the core business logic and rules. This layer has NO dependencies on any other layer or external frameworks.

**Components**:
- **Entities**: Core business objects (Project, Transaction, Person, BankAccount, AccountingAccount, AuditEntry)
- **Value Objects**: Immutable objects representing domain concepts (Money, ProjectStatus, TransactionClassification, AccountIdentifier)
- **Domain Services**: Business logic that doesn't naturally fit in entities (BalanceCalculationService, CryptographicService, DigitalSignatureService, IntegrityVerificationService)
- **Domain Events**: Events that represent something that happened in the domain

**Rules**:
- No dependencies on infrastructure or application layers
- Pure business logic only
- Framework-agnostic
- Highly testable in isolation

### 2. Application Layer (`ProjectBudgetManagement.Application`)

**Purpose**: Orchestrates the flow of data to and from the domain, and coordinates application-specific business rules.

**Components**:
- **Commands**: Write operations (CreateProjectCommand, CreateTransactionCommand, UpdateProjectCommand)
- **Queries**: Read operations (GetProjectQuery, GetTransactionHistoryQuery, GetAccountBalanceQuery)
- **Command/Query Handlers**: Execute commands and queries
- **Application Services**: Orchestrate domain logic (ProjectService, TransactionService, AuditService, ReportingService)
- **Ports (Interfaces)**: Define contracts for repositories and external services
  - **Input Ports**: Interfaces for use cases (commands/queries)
  - **Output Ports**: Repository interfaces (IProjectRepository, ITransactionRepository, etc.)

**Dependencies**:
- Depends on Domain layer only
- Defines interfaces that Infrastructure implements

### 3. Infrastructure Layer (`ProjectBudgetManagement.Infrastructure`)

**Purpose**: Implements the technical capabilities that support the higher layers. Contains all external concerns.

**Components**:
- **Persistence**: Entity Framework Core DbContext and entity configurations
- **Repositories**: Concrete implementations of repository interfaces defined in Application layer
- **External Services**: Implementations of external service interfaces
- **Database Migrations**: EF Core migrations for schema management

**Dependencies**:
- Depends on Domain and Application layers
- Implements interfaces defined in Application layer
- Contains framework-specific code (EF Core, SQL Server)

### 4. API Layer (`ProjectBudgetManagement.Api`)

**Purpose**: Exposes the application functionality through REST API endpoints. Acts as an adapter for HTTP requests.

**Components**:
- **Controllers**: REST API endpoints (ProjectsController, TransactionsController, etc.)
- **DTOs**: Data Transfer Objects for requests and responses
- **Middleware**: Cross-cutting concerns (authentication, error handling, logging)
- **Configuration**: Dependency injection setup, Swagger configuration

**Dependencies**:
- Depends on Application and Infrastructure layers
- Entry point of the application

## Dependency Flow

```
API Layer
    ↓ (depends on)
Application Layer
    ↓ (depends on)
Domain Layer
    ↑ (implemented by)
Infrastructure Layer
```

**Key Principle**: Dependencies point inward. The Domain layer has no dependencies. Infrastructure implements interfaces defined in Application.

## Design Patterns Used

### 1. Repository Pattern
- Abstracts data access logic
- Defined as interfaces in Application layer
- Implemented in Infrastructure layer
- Allows easy testing with mock repositories

### 2. Command Query Responsibility Segregation (CQRS)
- Separates read operations (Queries) from write operations (Commands)
- Improves scalability and maintainability
- Clear separation of concerns

### 3. Dependency Injection
- All dependencies injected through constructors
- Configured in Program.cs
- Enables loose coupling and testability

### 4. Service Layer Pattern
- Application services orchestrate domain logic
- Keep controllers thin
- Reusable business logic

## Project Structure

```
ProjectBudgetManagement/
├── src/
│   ├── ProjectBudgetManagement.Api/
│   │   ├── Controllers/          # REST API endpoints
│   │   ├── Models/               # DTOs (Request/Response)
│   │   ├── Middleware/           # Custom middleware
│   │   ├── Program.cs            # Application entry point
│   │   └── appsettings.json      # Configuration
│   │
│   ├── ProjectBudgetManagement.Application/
│   │   ├── Commands/             # Write operations
│   │   ├── Queries/              # Read operations
│   │   ├── Services/             # Application services
│   │   └── Ports/                # Repository interfaces
│   │
│   ├── ProjectBudgetManagement.Domain/
│   │   ├── Entities/             # Domain entities
│   │   ├── ValueObjects/         # Value objects
│   │   ├── Services/             # Domain services
│   │   └── Events/               # Domain events
│   │
│   └── ProjectBudgetManagement.Infrastructure/
│       ├── Persistence/          # EF Core DbContext
│       ├── Repositories/         # Repository implementations
│       └── Migrations/           # Database migrations
│
├── docker-compose.yml            # Docker orchestration
├── Dockerfile                    # API container definition
├── Makefile                      # Simplified commands
└── README.md                     # Project documentation
```

## Benefits of This Architecture

1. **Testability**: Domain logic can be tested without any infrastructure
2. **Maintainability**: Clear separation makes code easier to understand and modify
3. **Flexibility**: Easy to swap implementations (e.g., change database)
4. **Independence**: Business logic is independent of frameworks and UI
5. **Scalability**: CQRS pattern allows independent scaling of reads and writes

## Development Workflow

1. **Start with Domain**: Define entities, value objects, and domain services
2. **Define Application Layer**: Create commands, queries, and service interfaces
3. **Implement Infrastructure**: Create repositories and database configurations
4. **Build API Layer**: Create controllers and DTOs
5. **Wire Everything Together**: Configure dependency injection in Program.cs

## Testing Strategy

- **Unit Tests**: Test domain logic in isolation
- **Integration Tests**: Test with real database (SQL Server in Docker)
- **API Tests**: Test complete request/response flows
- **Performance Tests**: Ensure <100ms response time requirement

## Next Steps

1. Implement domain entities and value objects
2. Create repository interfaces in Application layer
3. Set up Entity Framework DbContext
4. Implement repositories in Infrastructure layer
5. Create application services
6. Build API controllers
7. Add authentication and authorization
8. Implement audit trail and cryptographic services
