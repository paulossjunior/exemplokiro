# Project Budget Management System

A financial accountability and expense tracking system built with .NET 8, following hexagonal architecture principles. The system enables project coordinators to manage projects with dedicated bank accounts, track financial transactions with complete audit trails, and ensure non-repudiation through cryptographic mechanisms.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Common Workflows](#common-workflows)
- [Security Features](#security-features)
- [Testing](#testing)
- [Troubleshooting](#troubleshooting)
- [Performance](#performance)
- [Development Guidelines](#development-guidelines)

## Features

- **Project Management**: Create and manage projects with budget allocation, timelines, and status tracking
- **Bank Account Integration**: Associate dedicated bank accounts with each project for financial segregation
- **Transaction Tracking**: Record all financial transactions (debits/credits) with accounting categorization
- **Audit Trail**: Maintain complete, immutable audit logs of all system activities
- **Digital Signatures**: Non-repudiation mechanisms linking transactions to authenticated users
- **Cryptographic Integrity**: SHA-256 hashing to detect data tampering
- **Accountability Reports**: Generate comprehensive reports with embedded audit trails and signatures
- **Balance Monitoring**: Real-time balance calculations with budget comparison

## Architecture

### System Overview

The system follows **Hexagonal Architecture (Ports and Adapters)** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                             │
│  (REST Controllers, DTOs, Request/Response Models)          │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                   Application Layer                          │
│  (Use Cases, Commands, Queries, Services)                   │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                     Domain Layer                             │
│  (Entities, Value Objects, Domain Services)                 │
└─────────────────────────────────────────────────────────────┘
                         ▲
┌────────────────────────┴────────────────────────────────────┐
│                 Infrastructure Layer                         │
│  (Repositories, EF Core, Cryptographic Services)            │
└─────────────────────────────────────────────────────────────┘
```

### Component Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                         Client                                │
└────────────────────────┬─────────────────────────────────────┘
                         │ HTTP/REST
┌────────────────────────▼─────────────────────────────────────┐
│                    API Controllers                            │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐       │
│  │Projects  │ │Transactions│ │Accounts │ │  Audit   │       │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘       │
└────────────────────────┬─────────────────────────────────────┘
                         │
┌────────────────────────▼─────────────────────────────────────┐
│                  Application Services                         │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐        │
│  │ProjectService│ │TransactionSvc│ │  AuditService │        │
│  └──────────────┘ └──────────────┘ └──────────────┘        │
└────────────────────────┬─────────────────────────────────────┘
                         │
┌────────────────────────▼─────────────────────────────────────┐
│                    Domain Services                            │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐        │
│  │BalanceCalc   │ │DigitalSign   │ │IntegrityVerif│        │
│  └──────────────┘ └──────────────┘ └──────────────┘        │
└────────────────────────┬─────────────────────────────────────┘
                         │
┌────────────────────────▼─────────────────────────────────────┐
│                     Repositories                              │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐       │
│  │Project   │ │Transaction│ │Account   │ │  Audit   │       │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘       │
└────────────────────────┬─────────────────────────────────────┘
                         │
┌────────────────────────▼─────────────────────────────────────┐
│                    SQL Server Database                        │
└──────────────────────────────────────────────────────────────┘
```

### Project Structure

```
src/
├── ProjectBudgetManagement.Api/          # API Layer
│   ├── Controllers/                      # REST API endpoints
│   ├── Models/                           # DTOs and request/response models
│   ├── Middleware/                       # Exception handling, performance monitoring
│   ├── Validators/                       # FluentValidation validators
│   └── Program.cs                        # Application entry point
│
├── ProjectBudgetManagement.Application/  # Application Layer
│   ├── Commands/                         # Write operations (CQRS)
│   ├── Queries/                          # Read operations (CQRS)
│   ├── Services/                         # Application orchestration
│   └── Ports/                            # Repository interfaces (output ports)
│
├── ProjectBudgetManagement.Domain/       # Domain Layer
│   ├── Entities/                         # Core business entities
│   ├── ValueObjects/                     # Immutable value objects
│   ├── Services/                         # Domain logic services
│   └── Exceptions/                       # Domain-specific exceptions
│
└── ProjectBudgetManagement.Infrastructure/ # Infrastructure Layer
    ├── Persistence/                      # EF Core DbContext
    ├── Repositories/                     # Repository implementations
    └── Services/                         # External service implementations

tests/
└── ProjectBudgetManagement.IntegrationTests/
    ├── Api/                              # API endpoint tests
    ├── Repositories/                     # Repository tests
    ├── Security/                         # Security feature tests
    ├── Performance/                      # Performance tests
    └── Infrastructure/                   # Test infrastructure
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

#### Development Mode (with hot reload)

```bash
# Start all services (API + SQL Server + Front-End Dev Server)
docker-compose up -d --build

# Stop services
docker-compose down

# View logs
docker-compose logs -f

# Access the application
# Front-End: http://localhost:5173
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

#### Production Mode

```bash
# Start all services with production configuration
docker-compose -f docker-compose.prod.yml up -d --build

# Stop services
docker-compose -f docker-compose.prod.yml down

# View logs
docker-compose -f docker-compose.prod.yml logs -f

# Access the application
# Front-End: http://localhost
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Local Development (without Docker)

```bash
# Restore dependencies
dotnet restore

# Run SQL Server in Docker
docker-compose up -d sqlserver

# Run the API locally
dotnet run --project src/ProjectBudgetManagement.Api

# In another terminal, run the front-end
cd front-end
npm install
npm run dev
```

### Front-End Only Development

If you want to work on the front-end with the back-end running in Docker:

```bash
# Start back-end services only
docker-compose up -d api sqlserver

# Run front-end locally with hot reload
cd front-end
npm install
npm run dev

# Access at http://localhost:5173
```

### Docker Build Details

For detailed information about the Docker setup, including:
- Multi-stage build process
- Build arguments and environment variables
- Security features
- Performance optimizations
- Troubleshooting

See: [front-end/DOCKER_BUILD.md](front-end/DOCKER_BUILD.md)

## API Documentation

Once the application is running, access the Swagger UI at:

```
http://localhost:5000/swagger
```

### API Endpoints

#### Projects

**Create Project**
```http
POST /api/projects
Content-Type: application/json

{
  "name": "Research Project 2025",
  "description": "Annual research initiative",
  "startDate": "2025-01-01",
  "endDate": "2025-12-31",
  "budgetAmount": 50000.00,
  "coordinatorName": "John Doe",
  "coordinatorIdentification": "12345678",
  "bankAccountNumber": "1234567890",
  "bankName": "National Bank",
  "branchNumber": "001"
}
```

**Response (201 Created)**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Research Project 2025",
  "description": "Annual research initiative",
  "startDate": "2025-01-01",
  "endDate": "2025-12-31",
  "status": "NotStarted",
  "budgetAmount": 50000.00,
  "coordinator": {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "name": "John Doe",
    "identificationNumber": "12345678"
  },
  "bankAccount": {
    "accountNumber": "1234567890",
    "bankName": "National Bank",
    "branchNumber": "001",
    "accountHolderName": "John Doe"
  }
}
```

**Get Project**
```http
GET /api/projects/{id}
```

**List Projects**
```http
GET /api/projects?status=InProgress&pageNumber=1&pageSize=10
```

**Update Project**
```http
PUT /api/projects/{id}
Content-Type: application/json

{
  "name": "Updated Project Name",
  "description": "Updated description",
  "endDate": "2025-12-31"
}
```

**Update Project Status**
```http
PUT /api/projects/{id}/status
Content-Type: application/json

{
  "status": "InProgress"
}
```

#### Transactions

**Create Transaction**
```http
POST /api/projects/{projectId}/transactions
Authorization: Bearer {jwt-token}
Content-Type: application/json

{
  "amount": 1500.00,
  "date": "2025-11-14",
  "classification": "Debit",
  "accountingAccountId": "8e9b7c6d-5a4f-3e2d-1c0b-9a8f7e6d5c4b",
  "description": "Office supplies purchase"
}
```

**Response (201 Created)**
```json
{
  "id": "9d8c7b6a-5f4e-3d2c-1b0a-9f8e7d6c5b4a",
  "amount": 1500.00,
  "date": "2025-11-14",
  "classification": "Debit",
  "accountingAccount": {
    "id": "8e9b7c6d-5a4f-3e2d-1c0b-9a8f7e6d5c4b",
    "name": "Office Supplies",
    "identifier": "6.2.2.1.01.04"
  },
  "digitalSignature": "HMAC-SHA256:a7f3b2c1...",
  "createdAt": "2025-11-14T10:30:00Z",
  "createdBy": "7c9e6679-7425-40de-944b-e07fc1f90ae7"
}
```

**Get Transaction History**
```http
GET /api/projects/{projectId}/transactions?startDate=2025-01-01&endDate=2025-12-31&classification=Debit
```

**Get Account Balance**
```http
GET /api/projects/{projectId}/balance
```

**Response (200 OK)**
```json
{
  "projectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "budgetAmount": 50000.00,
  "currentBalance": 48500.00,
  "totalCredits": 50000.00,
  "totalDebits": 1500.00,
  "isOverBudget": false,
  "calculatedAt": "2025-11-14T10:35:00Z"
}
```

#### Accounting Accounts

**Create Accounting Account**
```http
POST /api/accounting-accounts
Content-Type: application/json

{
  "name": "Office Supplies",
  "identifier": "6.2.2.1.01.04"
}
```

**List Accounting Accounts**
```http
GET /api/accounting-accounts
```

**Get Accounting Account**
```http
GET /api/accounting-accounts/{id}
```

#### Audit Trail

**Get Audit Trail**
```http
GET /api/audit/trail?entityType=Transaction&startDate=2025-01-01&endDate=2025-12-31
```

**Response (200 OK)**
```json
{
  "entries": [
    {
      "id": "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d",
      "userId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
      "actionType": "Create",
      "entityType": "Transaction",
      "entityId": "9d8c7b6a-5f4e-3d2c-1b0a-9f8e7d6c5b4a",
      "timestamp": "2025-11-14T10:30:00Z",
      "previousValue": null,
      "newValue": "{\"amount\":1500.00,\"classification\":\"Debit\"}",
      "digitalSignature": "HMAC-SHA256:b8e4c3d2...",
      "dataHash": "SHA256:9f7e6d5c..."
    }
  ],
  "totalCount": 1
}
```

**Verify Data Integrity**
```http
GET /api/audit/integrity
```

**Response (200 OK)**
```json
{
  "isValid": true,
  "totalRecordsChecked": 150,
  "invalidRecords": [],
  "verifiedAt": "2025-11-14T10:40:00Z"
}
```

#### Reports

**Generate Accountability Report**
```http
POST /api/reports/accountability/{projectId}
Content-Type: application/json

{
  "includeAuditTrail": true,
  "startDate": "2025-01-01",
  "endDate": "2025-12-31"
}
```

**Response (200 OK)**
```json
{
  "reportId": "5f6e7d8c-9b0a-1f2e-3d4c-5b6a7f8e9d0c",
  "projectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "generatedAt": "2025-11-14T10:45:00Z",
  "downloadUrl": "/api/reports/5f6e7d8c-9b0a-1f2e-3d4c-5b6a7f8e9d0c/download"
}
```

**Download Report**
```http
GET /api/reports/{reportId}/download
```

Returns PDF file with accountability report.

## Common Workflows

### Workflow 1: Create a New Project

```bash
# 1. Create a project with coordinator and bank account
curl -X POST http://localhost:5000/api/projects \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Research Project 2025",
    "description": "Annual research initiative",
    "startDate": "2025-01-01",
    "endDate": "2025-12-31",
    "budgetAmount": 50000.00,
    "coordinatorName": "John Doe",
    "coordinatorIdentification": "12345678",
    "bankAccountNumber": "1234567890",
    "bankName": "National Bank",
    "branchNumber": "001"
  }'

# Response includes project ID: "3fa85f64-5717-4562-b3fc-2c963f66afa6"

# 2. Update project status to "InProgress"
curl -X PUT http://localhost:5000/api/projects/3fa85f64-5717-4562-b3fc-2c963f66afa6/status \
  -H "Content-Type: application/json" \
  -d '{"status": "InProgress"}'
```

### Workflow 2: Record Transactions

```bash
# 1. Create an accounting account (if not exists)
curl -X POST http://localhost:5000/api/accounting-accounts \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Office Supplies",
    "identifier": "6.2.2.1.01.04"
  }'

# Response includes account ID: "8e9b7c6d-5a4f-3e2d-1c0b-9a8f7e6d5c4b"

# 2. Create a debit transaction (requires authentication)
curl -X POST http://localhost:5000/api/projects/3fa85f64-5717-4562-b3fc-2c963f66afa6/transactions \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 1500.00,
    "date": "2025-11-14",
    "classification": "Debit",
    "accountingAccountId": "8e9b7c6d-5a4f-3e2d-1c0b-9a8f7e6d5c4b",
    "description": "Office supplies purchase"
  }'

# 3. Check account balance
curl -X GET http://localhost:5000/api/projects/3fa85f64-5717-4562-b3fc-2c963f66afa6/balance
```

### Workflow 3: Generate Accountability Report

```bash
# 1. Generate report for a project
curl -X POST http://localhost:5000/api/reports/accountability/3fa85f64-5717-4562-b3fc-2c963f66afa6 \
  -H "Content-Type: application/json" \
  -d '{
    "includeAuditTrail": true,
    "startDate": "2025-01-01",
    "endDate": "2025-12-31"
  }'

# Response includes report ID: "5f6e7d8c-9b0a-1f2e-3d4c-5b6a7f8e9d0c"

# 2. Download the PDF report
curl -X GET http://localhost:5000/api/reports/5f6e7d8c-9b0a-1f2e-3d4c-5b6a7f8e9d0c/download \
  -o accountability-report.pdf
```

### Workflow 4: Audit Trail Review

```bash
# 1. Get audit trail for all transactions
curl -X GET "http://localhost:5000/api/audit/trail?entityType=Transaction&startDate=2025-01-01&endDate=2025-12-31"

# 2. Verify data integrity
curl -X GET http://localhost:5000/api/audit/integrity

# 3. Get transaction history with filters
curl -X GET "http://localhost:5000/api/projects/3fa85f64-5717-4562-b3fc-2c963f66afa6/transactions?classification=Debit&startDate=2025-01-01"
```

## Security Features

### Cryptographic Mechanisms

The system implements multiple layers of security to ensure data integrity and non-repudiation:

#### 1. Data Integrity Hashing (SHA-256)

Every transaction and audit entry includes a cryptographic hash to detect tampering:

**Transaction Hash Calculation**
```
DataHash = SHA256(
  Amount + 
  Date + 
  Classification + 
  BankAccountId + 
  AccountingAccountId + 
  CreatedBy
)
```

**Audit Entry Hash Calculation**
```
DataHash = SHA256(
  UserId + 
  ActionType + 
  EntityType + 
  EntityId + 
  Timestamp + 
  PreviousValue + 
  NewValue
)
```

**Verification Process**
- On every read operation, the system recalculates the hash
- Compares calculated hash with stored hash
- If mismatch detected, logs security alert and prevents access
- Integrity verification endpoint checks all records

#### 2. Digital Signatures

Transactions are digitally signed to ensure non-repudiation:

**Signature Generation**
```
DigitalSignature = HMAC-SHA256(
  TransactionData,
  UserSecret
)
```

**Properties**
- Links transaction to authenticated user
- Cannot be forged without user's secret key
- Proves coordinator authorized the transaction
- Stored immutably with transaction record

#### 3. Immutable Audit Trail

**Characteristics**
- Audit entries cannot be modified or deleted
- Database constraints prevent updates
- Every system action is logged
- Includes user identification and timestamps
- Cryptographically protected with hashes and signatures

**Logged Actions**
- Project creation, updates, status changes
- Transaction creation
- Accounting account creation
- Report generation
- Failed authentication attempts

#### 4. Authentication & Authorization

**JWT Token Authentication**
- Tokens issued by authentication service
- Include user ID and roles
- Expire after configurable period
- Required for transaction creation

**Authorization Rules**
- Only assigned coordinator can create transactions for their project
- Audit trail access restricted to authorized users
- Report generation requires appropriate permissions

#### 5. Security Headers

The API implements security best practices:
- **HTTPS**: Enforced in production
- **HSTS**: HTTP Strict Transport Security
- **CSP**: Content Security Policy
- **X-Frame-Options**: Prevents clickjacking
- **X-Content-Type-Options**: Prevents MIME sniffing

### Security Validation Example

```bash
# 1. Create a transaction (generates hash and signature)
curl -X POST http://localhost:5000/api/projects/{projectId}/transactions \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"amount": 1000, "date": "2025-11-14", "classification": "Debit", "accountingAccountId": "..."}'

# 2. Verify integrity of all records
curl -X GET http://localhost:5000/api/audit/integrity

# Response shows validation status
{
  "isValid": true,
  "totalRecordsChecked": 150,
  "invalidRecords": [],
  "verifiedAt": "2025-11-14T10:40:00Z"
}

# 3. If tampering detected, system returns:
{
  "isValid": false,
  "totalRecordsChecked": 150,
  "invalidRecords": [
    {
      "entityType": "Transaction",
      "entityId": "...",
      "issue": "Hash mismatch - possible tampering detected"
    }
  ],
  "verifiedAt": "2025-11-14T10:40:00Z"
}
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

## Testing

### Running Tests

The project includes comprehensive integration tests that use a real SQL Server database.

```bash
# Run all tests using Makefile
make test

# Run tests using dotnet CLI
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# Run specific test category
dotnet test --filter "Category=Integration"

# Run performance tests
dotnet test --filter "FullyQualifiedName~Performance"
```

### Test Structure

```
tests/ProjectBudgetManagement.IntegrationTests/
├── Api/                              # API endpoint tests
│   ├── ProjectsControllerTests.cs
│   ├── TransactionsControllerTests.cs
│   ├── AccountingAccountsControllerTests.cs
│   └── AuditControllerTests.cs
│
├── Repositories/                     # Repository integration tests
│   ├── ProjectRepositoryTests.cs
│   ├── TransactionRepositoryTests.cs
│   └── DatabaseConstraintsTests.cs
│
├── Security/                         # Security feature tests
│   └── SecurityIntegrationTests.cs
│
├── Performance/                      # Performance tests
│   └── PerformanceTests.cs
│
├── EndToEnd/                         # End-to-end workflow tests
│   └── RequirementsVerificationTests.cs
│
└── Infrastructure/                   # Test infrastructure
    ├── IntegrationTestBase.cs
    ├── DatabaseFixture.cs
    └── IntegrationTestWebAppFactory.cs
```

### Test Database Setup

Tests automatically:
1. Spin up SQL Server container (if using Docker)
2. Apply EF Core migrations
3. Run tests with transaction-based isolation
4. Rollback after each test for clean state

**Manual Database Setup (if needed)**
```bash
# Start SQL Server for testing
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name sqlserver-test \
  -d mcr.microsoft.com/mssql/server:2022-latest

# Run tests
dotnet test
```

### Test Categories

**API Integration Tests**
- Test all REST endpoints
- Validate request/response formats
- Test error handling and status codes
- Verify authentication and authorization

**Repository Tests**
- Test CRUD operations with real database
- Validate database constraints
- Test complex queries and joins
- Verify transaction isolation

**Security Tests**
- Hash computation and verification
- Digital signature generation and validation
- Tampering detection
- Audit trail immutability
- Authorization enforcement

**Performance Tests**
- Validate <100ms response time requirement
- Load testing with realistic data volumes
- Concurrent user scenarios
- Query optimization verification

**End-to-End Tests**
- Complete workflow scenarios
- Requirements verification
- Integration between components

### Running Specific Tests

```bash
# Run only API tests
dotnet test --filter "FullyQualifiedName~Api"

# Run only security tests
dotnet test --filter "FullyQualifiedName~Security"

# Run only performance tests
dotnet test --filter "FullyQualifiedName~Performance"

# Run a specific test
dotnet test --filter "FullyQualifiedName~CreateProject_WithValidData_ReturnsCreatedProject"
```

### Test Results

Performance test results are documented in:
```
tests/ProjectBudgetManagement.IntegrationTests/Performance/PERFORMANCE_TEST_RESULTS.md
```

## Troubleshooting

### Common Issues

#### 1. Database Connection Failures

**Symptom**: `Cannot connect to SQL Server` or `Login failed for user 'sa'`

**Solutions**:
```bash
# Check if SQL Server container is running
docker ps | grep sqlserver

# Restart SQL Server container
docker-compose restart sqlserver

# Check SQL Server logs
docker-compose logs sqlserver

# Verify connection string in appsettings.json
# Ensure password matches docker-compose.yml
```

#### 2. Port Already in Use

**Symptom**: `Address already in use` or `Port 5000 is already allocated`

**Solutions**:
```bash
# Find process using port 5000
lsof -i :5000  # macOS/Linux
netstat -ano | findstr :5000  # Windows

# Kill the process or change port in appsettings.json
# Update "Urls": "http://localhost:5001" in appsettings.json

# Or stop conflicting containers
docker-compose down
```

#### 3. Migration Errors

**Symptom**: `Pending model changes` or `Migration failed`

**Solutions**:
```bash
# Drop database and recreate
docker-compose down -v
docker-compose up -d

# Or manually reset database
dotnet ef database drop --project src/ProjectBudgetManagement.Infrastructure --startup-project src/ProjectBudgetManagement.Api --force
dotnet ef database update --project src/ProjectBudgetManagement.Infrastructure --startup-project src/ProjectBudgetManagement.Api

# Check for pending migrations
dotnet ef migrations list --project src/ProjectBudgetManagement.Infrastructure --startup-project src/ProjectBudgetManagement.Api
```

#### 4. Docker Build Failures

**Symptom**: `Docker build failed` or `Cannot find project file`

**Solutions**:
```bash
# Clean Docker cache
docker system prune -a

# Rebuild without cache
docker-compose build --no-cache

# Check Dockerfile paths are correct
# Ensure .dockerignore is not excluding necessary files
```

#### 5. Test Failures

**Symptom**: Tests fail with database errors or timeout

**Solutions**:
```bash
# Ensure test database is running
docker ps | grep sqlserver

# Check connection string in test configuration
# Default: Server=localhost,1433;Database=ProjectBudgetManagement_Test;...

# Run tests with verbose output to see details
dotnet test --verbosity detailed

# Clean and rebuild test project
dotnet clean
dotnet build
dotnet test
```

#### 6. Swagger Not Loading

**Symptom**: `404 Not Found` when accessing `/swagger`

**Solutions**:
```bash
# Verify Swagger is enabled in Program.cs
# Check environment is Development or Swagger is enabled for all environments

# Access correct URL
http://localhost:5000/swagger/index.html

# Check application logs
docker-compose logs api
```

#### 7. Authentication Errors

**Symptom**: `401 Unauthorized` when creating transactions

**Solutions**:
```bash
# Ensure JWT token is included in Authorization header
curl -H "Authorization: Bearer YOUR_TOKEN" ...

# Verify token is not expired
# Check token includes required claims (user ID, roles)

# For testing, you may need to implement token generation endpoint
# Or temporarily disable authentication in development
```

#### 8. Performance Issues

**Symptom**: API responses slower than 100ms requirement

**Solutions**:
```bash
# Check database indexes are created
# Review query execution plans

# Enable query logging in appsettings.json
"Logging": {
  "LogLevel": {
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
  }
}

# Monitor performance metrics endpoint
curl http://localhost:5000/api/metrics

# Review performance test results
cat tests/ProjectBudgetManagement.IntegrationTests/Performance/PERFORMANCE_TEST_RESULTS.md
```

#### 9. Data Integrity Errors

**Symptom**: `Hash mismatch` or `Integrity verification failed`

**Solutions**:
```bash
# This indicates potential data tampering or corruption
# Check audit trail for suspicious activity
curl http://localhost:5000/api/audit/trail

# Run integrity verification
curl http://localhost:5000/api/audit/integrity

# Review security logs
docker-compose logs api | grep "Security"

# If in development, may need to recreate database
docker-compose down -v
docker-compose up -d
```

### Getting Help

If you encounter issues not covered here:

1. Check application logs: `docker-compose logs api`
2. Check database logs: `docker-compose logs sqlserver`
3. Review test output: `dotnet test --verbosity detailed`
4. Check the design document: `.kiro/specs/project-budget-management/design.md`
5. Review requirements: `.kiro/specs/project-budget-management/requirements.md`

### Debug Mode

Run the application in debug mode for detailed logging:

```bash
# Set environment to Development
export ASPNETCORE_ENVIRONMENT=Development

# Run with detailed logging
dotnet run --project src/ProjectBudgetManagement.Api --verbosity detailed
```

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

## Performance

### Performance Requirements

All API endpoints must respond in **less than 100ms** as per the design specifications.

### Performance Optimization Strategies

**Database Optimization**
- Indexes on frequently queried fields (Status, Date, BankAccountId, EntityId)
- Compiled queries for repeated operations
- AsNoTracking() for read-only queries
- Connection pooling enabled

**Caching Strategy**
- Cache accounting accounts (rarely change)
- Cache project details for balance calculations
- Distributed cache support for scalability

**Query Optimization**
- Eager loading with Include() to prevent N+1 queries
- Pagination for large result sets
- Projection queries to minimize data transfer

### Performance Monitoring

```bash
# Access performance metrics endpoint
curl http://localhost:5000/api/metrics

# Response includes:
{
  "averageResponseTime": 45.2,
  "requestsPerSecond": 120,
  "activeConnections": 15,
  "databaseQueryTime": 12.5
}
```

### Performance Test Results

Performance tests validate the <100ms requirement with realistic data volumes:
- 1000+ projects
- 10,000+ transactions
- 100+ concurrent users

Results documented in: `tests/ProjectBudgetManagement.IntegrationTests/Performance/PERFORMANCE_TEST_RESULTS.md`

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
