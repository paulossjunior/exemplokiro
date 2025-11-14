# Project Budget Management System

A financial accountability and expense tracking system built with .NET 8, following hexagonal architecture principles.

## Features

- Project lifecycle management with budget allocation
- Bank account association and transaction management
- Accounting account categorization
- Complete audit trail with digital signatures
- Non-repudiation mechanisms for financial transactions
- Accountability reporting with cryptographic integrity verification

## Architecture

The system follows **Hexagonal Architecture (Ports and Adapters)** with clear separation of concerns:

```
src/
├── ProjectBudgetManagement.Api/          # API Layer (Controllers, DTOs)
├── ProjectBudgetManagement.Application/  # Application Layer (Use Cases, Services)
├── ProjectBudgetManagement.Domain/       # Domain Layer (Entities, Value Objects)
└── ProjectBudgetManagement.Infrastructure/ # Infrastructure Layer (Repositories, EF Core)
```

## Technology Stack

- **.NET 8** - Framework
- **ASP.NET Core** - Web API
- **Entity Framework Core 8** - ORM
- **SQL Server** - Database
- **Docker** - Containerization
- **Swagger/OpenAPI** - API Documentation

## Prerequisites

- .NET 8 SDK
- Docker Desktop
- Make (optional, for simplified commands)

## Getting Started

### Using Makefile (Recommended)

```bash
# Start all services (API + SQL Server)
make up

# View logs
make logs

# Stop services
make down

# Rebuild and restart
make rebuild

# Clean up (remove volumes)
make clean
```

### Using Docker Compose Directly

```bash
# Start services
docker-compose up -d --build

# Stop services
docker-compose down

# View logs
docker-compose logs -f
```

### Local Development (without Docker)

```bash
# Restore dependencies
dotnet restore

# Run SQL Server in Docker
docker-compose up -d sqlserver

# Run the API locally
dotnet run --project src/ProjectBudgetManagement.Api
```

## API Documentation

Once the application is running, access the Swagger UI at:

```
http://localhost:5000/swagger
```

## Database Migrations

```bash
# Add a new migration
make migrate-add NAME=InitialCreate

# Apply migrations
make migrate-up
```

Or using dotnet CLI directly:

```bash
# Add migration
dotnet ef migrations add InitialCreate --project src/ProjectBudgetManagement.Infrastructure --startup-project src/ProjectBudgetManagement.Api

# Update database
dotnet ef database update --project src/ProjectBudgetManagement.Infrastructure --startup-project src/ProjectBudgetManagement.Api
```

## Project Structure

### Domain Layer
- **Entities**: Core business entities (Project, Transaction, Person, etc.)
- **Value Objects**: Immutable objects (Money, ProjectStatus, etc.)
- **Services**: Domain logic services (BalanceCalculationService, CryptographicService)

### Application Layer
- **Commands**: Write operations (CreateProject, CreateTransaction)
- **Queries**: Read operations (GetProject, GetTransactionHistory)
- **Services**: Application orchestration services
- **Ports**: Repository interfaces (output ports)

### Infrastructure Layer
- **Persistence**: EF Core DbContext and configurations
- **Repositories**: Repository implementations
- **External Services**: Third-party integrations

### API Layer
- **Controllers**: REST API endpoints
- **Models**: DTOs for requests and responses
- **Middleware**: Cross-cutting concerns

## Configuration

### Connection String

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ProjectBudgetManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

### Environment Variables

When running in Docker, environment variables are configured in `docker-compose.yml`.

## Performance Requirements

All API endpoints must respond in **less than 100ms** as per the design specifications.

## Security

- HTTPS enforced in production
- Security headers configured (HSTS, CSP, X-Frame-Options)
- JWT authentication for protected endpoints
- Cryptographic hashing (SHA-256) for data integrity
- Digital signatures for non-repudiation

## Testing

```bash
# Run tests (once test project is created)
make test
```

## Development Guidelines

- Follow SOLID principles
- Use dependency injection for all dependencies
- Write tests before implementation (TDD)
- Keep code in English (comments, variables, documentation)
- Follow hexagonal architecture boundaries

## License

[Add your license here]

## Contributing

[Add contribution guidelines here]
