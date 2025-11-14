---
inclusion: manual
---

# Back-End Technology Stack

## Tech Stack

- **Language**: C#
- **Framework**: .NET 8
- **Runtime**: .NET 8 Runtime
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Containerization**: Docker
- **API Documentation**: Swagger/OpenAPI
- **API Design**: API First approach

## Build System

- .NET CLI for building and testing
- Docker for containerization
- Docker Compose for multi-container orchestration
- **Makefile** for simplified Docker command execution

## Common Commands

Use Makefile targets to simplify Docker operations:

```bash
# Build and run services
make up

# Stop services
make down

# Run tests in Docker
make test

# View logs
make logs

# Rebuild containers
make rebuild

# Database migrations
make migrate-add NAME=<MigrationName>
make migrate-up

# Clean up containers and volumes
make clean
```

### Direct Commands (if not using Makefile)

```bash
# Build
dotnet build

# Run
dotnet run

# Test (inside Docker)
docker-compose up --build test

# Database migrations
dotnet ef migrations add <MigrationName>
dotnet ef database update

# Docker operations
docker-compose up -d          # Start services
docker-compose down           # Stop services
docker-compose logs -f        # View logs
```

## Development Setup

### Prerequisites

- .NET 8 SDK
- Docker Desktop
- SQL Server (via Docker)

### Local Development

1. Restore dependencies: `dotnet restore`
2. Update database: `dotnet ef database update`
3. Run application: `dotnet run`

## Database Configuration

- Use Entity Framework Core for all database operations
- Migrations should be version controlled
- Connection strings configured via environment variables
- SQL Server runs in Docker container for development
