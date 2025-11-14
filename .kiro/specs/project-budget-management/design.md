# Design Document - Project Budget Management System

## 1. Introduction

### 1.1 Purpose

This document describes the software design for the Project Budget Management System, a financial accountability platform that enables project coordinators to manage projects with dedicated bank accounts and track financial transactions with complete audit trails and non-repudiation guarantees.

### 1.2 Scope

The system provides:
- Project lifecycle management with budget allocation
- Bank account association and transaction management
- Accounting account categorization
- Complete audit trail with digital signatures
- Non-repudiation mechanisms for financial transactions
- Accountability reporting with cryptographic integrity verification

### 1.3 Design Goals

- **Security**: Ensure data integrity and non-repudiation through cryptographic mechanisms
- **Auditability**: Maintain complete, immutable audit trails for all operations
- **Performance**: All API endpoints respond in less than 100ms
- **Maintainability**: Follow hexagonal architecture for clear separation of concerns
- **Testability**: Design for comprehensive testing with real database


## 2. System Overview

### 2.1 High-Level Architecture

The system follows **Hexagonal Architecture (Ports and Adapters)** with clear separation between domain logic, application services, and infrastructure concerns.

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                             │
│  (REST Controllers, DTOs, Request/Response Models)          │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                   Application Layer                          │
│  (Use Cases, Commands, Queries, Input Ports)                │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                     Domain Layer                             │
│  (Entities, Value Objects, Domain Services, Events)         │
└─────────────────────────────────────────────────────────────┘
                         ▲
┌────────────────────────┴────────────────────────────────────┐
│                 Infrastructure Layer                         │
│  (Repositories, EF Core, External Services, Crypto)         │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Technology Stack

- **Framework**: .NET 8 with ASP.NET Core
- **Database**: SQL Server with Entity Framework Core
- **API**: RESTful API with Swagger/OpenAPI documentation
- **Security**: Cryptographic hashing (SHA-256) for data integrity
- **Authentication**: JWT tokens for user authentication
- **Containerization**: Docker with Docker Compose


## 3. Architectural Design

### 3.1 Domain Layer

The domain layer contains pure business logic with no external dependencies.

#### 3.1.1 Domain Entities

**Project**
- Properties: Id, Name, Description, StartDate, EndDate, Status, BudgetAmount
- Relationships: Has one BankAccount, has one ProjectCoordinator (Person), has many Transactions
- Business Rules:
  - StartDate must be <= EndDate
  - BudgetAmount must be > 0
  - Status transitions follow defined workflow
  - Cannot create transactions when status is Completed or Cancelled

**Person**
- Properties: Id, Name, IdentificationNumber
- Relationships: Can coordinate multiple Projects
- Business Rules:
  - IdentificationNumber must be unique
  - Name is required

**BankAccount**
- Properties: Id, AccountNumber, BankName, BranchNumber, AccountHolderName
- Relationships: Belongs to one Project, has many Transactions
- Business Rules:
  - Combination of AccountNumber, BankName, BranchNumber must be unique
  - AccountNumber and BranchNumber must be numeric
  - AccountHolderName must match Project Coordinator name

**Transaction**
- Properties: Id, Amount, Date, Classification (Debit/Credit), DigitalSignature
- Relationships: Belongs to one BankAccount, belongs to one AccountingAccount, has one AuditEntry
- Business Rules:
  - Amount must be > 0
  - Date cannot be in the future
  - Cannot be modified or deleted after creation
  - Must have valid digital signature from Project Coordinator

**AccountingAccount**
- Properties: Id, Name, Identifier
- Relationships: Has many Transactions
- Business Rules:
  - Identifier must be unique and follow format pattern
  - Cannot be deleted if has associated Transactions

**AuditEntry**
- Properties: Id, UserId, ActionType, EntityType, EntityId, Timestamp, PreviousValue, NewValue, DigitalSignature, DataHash
- Relationships: Immutable record
- Business Rules:
  - Cannot be modified or deleted
  - Must have cryptographic hash for integrity verification
  - Must include digital signature


#### 3.1.2 Value Objects

**ProjectStatus** (Enum)
- Values: NotStarted, Initiated, InProgress, Completed, Cancelled

**TransactionClassification** (Enum)
- Values: Debit, Credit

**Money** (Value Object)
- Properties: Amount (decimal), Currency (string)
- Validation: Amount must be positive

**AccountIdentifier** (Value Object)
- Properties: Value (string)
- Validation: Must match pattern (e.g., "XXXX.XX.XXXX")

#### 3.1.3 Domain Services

**BalanceCalculationService**
- Calculates current balance from transaction history
- Compares balance against budget
- Generates balance warnings

**IntegrityVerificationService**
- Validates cryptographic hashes of records
- Detects tampering attempts
- Generates integrity reports

**DigitalSignatureService**
- Creates digital signatures for transactions
- Validates signatures for non-repudiation
- Links signatures to authenticated users

### 3.2 Application Layer

#### 3.2.1 Use Cases (Commands)

**Project Management**
- CreateProjectCommand
- UpdateProjectCommand
- UpdateProjectStatusCommand
- GetProjectQuery
- ListProjectsQuery

**Transaction Management**
- CreateTransactionCommand (requires authentication + signature)
- GetTransactionHistoryQuery
- GetAccountBalanceQuery

**Accounting**
- CreateAccountingAccountCommand
- ListAccountingAccountsQuery

**Audit & Reporting**
- GetAuditTrailQuery
- GenerateAccountabilityReportCommand
- VerifyDataIntegrityQuery


#### 3.2.2 Application Services

**ProjectService**
- Orchestrates project creation and updates
- Validates business rules
- Triggers audit logging

**TransactionService**
- Handles transaction creation with digital signatures
- Validates coordinator authorization
- Ensures audit trail creation
- Prevents transactions on closed projects

**AuditService**
- Records all system actions
- Generates immutable audit entries
- Creates cryptographic hashes for integrity

**ReportingService**
- Generates accountability reports
- Includes audit trail and signatures
- Exports to PDF format
- Embeds integrity verification data

### 3.3 Infrastructure Layer

#### 3.3.1 Persistence

**Entity Framework Core Configuration**
- DbContext: ProjectBudgetDbContext
- Migrations for schema management
- Indexes on frequently queried fields:
  - Project.Status
  - Transaction.Date
  - Transaction.BankAccountId
  - AuditEntry.Timestamp
  - AuditEntry.EntityId

**Repository Implementations**
- IProjectRepository → ProjectRepository
- ITransactionRepository → TransactionRepository
- IAccountingAccountRepository → AccountingAccountRepository
- IAuditRepository → AuditRepository

#### 3.3.2 Security Infrastructure

**CryptographicService**
- SHA-256 hashing for data integrity
- Digital signature generation and validation
- Hash verification for audit entries

**AuthenticationService**
- JWT token validation
- User identity extraction
- Coordinator authorization checks


### 3.4 API Layer

#### 3.4.1 REST Controllers

**ProjectsController**
- POST /api/projects - Create project
- GET /api/projects/{id} - Get project details
- GET /api/projects - List projects
- PUT /api/projects/{id} - Update project
- PUT /api/projects/{id}/status - Update status

**TransactionsController**
- POST /api/projects/{projectId}/transactions - Create transaction (requires auth)
- GET /api/projects/{projectId}/transactions - Get transaction history
- GET /api/projects/{projectId}/balance - Get current balance

**AccountingAccountsController**
- POST /api/accounting-accounts - Create accounting account
- GET /api/accounting-accounts - List accounting accounts
- GET /api/accounting-accounts/{id} - Get accounting account details

**AuditController**
- GET /api/audit/trail - Get audit trail (with filters)
- GET /api/audit/integrity - Verify data integrity

**ReportsController**
- POST /api/reports/accountability/{projectId} - Generate accountability report
- GET /api/reports/{reportId}/download - Download report PDF

#### 3.4.2 DTOs (Data Transfer Objects)

**Request DTOs**
- CreateProjectRequest
- UpdateProjectRequest
- CreateTransactionRequest
- CreateAccountingAccountRequest
- GenerateReportRequest

**Response DTOs**
- ProjectResponse
- TransactionResponse
- AccountBalanceResponse
- AuditEntryResponse
- IntegrityReportResponse
- AccountabilityReportResponse


## 4. Data Models

### 4.1 Database Schema

```sql
-- Projects Table
CREATE TABLE Projects (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Status INT NOT NULL,
    BudgetAmount DECIMAL(18,2) NOT NULL,
    CoordinatorId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_Projects_Coordinators FOREIGN KEY (CoordinatorId) REFERENCES Persons(Id),
    CONSTRAINT CK_Projects_Dates CHECK (StartDate <= EndDate),
    CONSTRAINT CK_Projects_Budget CHECK (BudgetAmount > 0)
);

-- Persons Table
CREATE TABLE Persons (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    IdentificationNumber NVARCHAR(50) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL
);

-- BankAccounts Table
CREATE TABLE BankAccounts (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    AccountNumber NVARCHAR(20) NOT NULL,
    BankName NVARCHAR(100) NOT NULL,
    BranchNumber NVARCHAR(10) NOT NULL,
    AccountHolderName NVARCHAR(200) NOT NULL,
    ProjectId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_BankAccounts_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
    CONSTRAINT UQ_BankAccounts UNIQUE (AccountNumber, BankName, BranchNumber)
);

-- AccountingAccounts Table
CREATE TABLE AccountingAccounts (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Identifier NVARCHAR(50) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL
);

-- Transactions Table
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Amount DECIMAL(18,2) NOT NULL,
    Date DATE NOT NULL,
    Classification INT NOT NULL,
    DigitalSignature NVARCHAR(500) NOT NULL,
    DataHash NVARCHAR(64) NOT NULL,
    BankAccountId UNIQUEIDENTIFIER NOT NULL,
    AccountingAccountId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_Transactions_BankAccounts FOREIGN KEY (BankAccountId) REFERENCES BankAccounts(Id),
    CONSTRAINT FK_Transactions_AccountingAccounts FOREIGN KEY (AccountingAccountId) REFERENCES AccountingAccounts(Id),
    CONSTRAINT CK_Transactions_Amount CHECK (Amount > 0)
);

-- AuditEntries Table
CREATE TABLE AuditEntries (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    ActionType NVARCHAR(50) NOT NULL,
    EntityType NVARCHAR(100) NOT NULL,
    EntityId UNIQUEIDENTIFIER NOT NULL,
    Timestamp DATETIME2 NOT NULL,
    PreviousValue NVARCHAR(MAX),
    NewValue NVARCHAR(MAX),
    DigitalSignature NVARCHAR(500) NOT NULL,
    DataHash NVARCHAR(64) NOT NULL
);

-- Indexes for performance
CREATE INDEX IX_Projects_Status ON Projects(Status);
CREATE INDEX IX_Transactions_Date ON Transactions(Date);
CREATE INDEX IX_Transactions_BankAccountId ON Transactions(BankAccountId);
CREATE INDEX IX_AuditEntries_Timestamp ON AuditEntries(Timestamp);
CREATE INDEX IX_AuditEntries_EntityId ON AuditEntries(EntityId);
```


### 4.2 Entity Relationships

```
Person (1) ──────< (N) Project
Project (1) ────── (1) BankAccount
BankAccount (1) ──< (N) Transaction
AccountingAccount (1) ──< (N) Transaction
Transaction (1) ──── (1) AuditEntry
```

### 4.3 Data Integrity Mechanisms

**Cryptographic Hashing**
- Each Transaction has a DataHash computed from: Amount + Date + Classification + BankAccountId + AccountingAccountId + CreatedBy
- Each AuditEntry has a DataHash computed from: UserId + ActionType + EntityType + EntityId + Timestamp + PreviousValue + NewValue
- Hash algorithm: SHA-256
- Hashes are verified on read operations to detect tampering

**Digital Signatures**
- Transactions include DigitalSignature linking to authenticated user
- Signature format: HMAC-SHA256(TransactionData, UserSecret)
- AuditEntries include DigitalSignature for non-repudiation
- Signatures are validated before accepting operations

## 5. Component Interactions

### 5.1 Create Transaction Flow

```
1. User (Coordinator) → POST /api/projects/{id}/transactions
2. TransactionsController → Validates JWT token
3. TransactionsController → Extracts user identity
4. TransactionsController → CreateTransactionCommand
5. TransactionService → Validates coordinator authorization
6. TransactionService → Validates project status (not Completed/Cancelled)
7. TransactionService → Creates Transaction entity
8. DigitalSignatureService → Generates signature
9. CryptographicService → Computes data hash
10. TransactionRepository → Saves transaction
11. AuditService → Creates audit entry with signature and hash
12. AuditRepository → Saves audit entry
13. TransactionsController → Returns TransactionResponse
```

### 5.2 Generate Accountability Report Flow

```
1. User → POST /api/reports/accountability/{projectId}
2. ReportsController → GenerateAccountabilityReportCommand
3. ReportingService → Retrieves project details
4. ReportingService → Retrieves all transactions
5. ReportingService → Retrieves audit trail
6. BalanceCalculationService → Calculates current balance
7. IntegrityVerificationService → Verifies all data hashes
8. ReportingService → Compiles report data
9. ReportingService → Generates PDF with embedded signatures
10. ReportingService → Stores report with unique identifier
11. ReportsController → Returns report metadata with download link
```


## 6. Error Handling

### 6.1 Error Categories

**Validation Errors (400 Bad Request)**
- Invalid input data (negative amounts, future dates)
- Business rule violations (start date after end date)
- Format errors (invalid account identifiers)

**Authorization Errors (401/403)**
- Missing or invalid JWT token
- Coordinator attempting to create transaction for another project
- Unauthorized access to audit trail

**Not Found Errors (404)**
- Project, transaction, or account not found
- Invalid entity identifiers

**Conflict Errors (409)**
- Duplicate bank account
- Duplicate accounting account identifier
- Attempting to create transaction on closed project

**Integrity Errors (500)**
- Data hash verification failure
- Digital signature validation failure
- Database constraint violations

### 6.2 Error Response Format

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "The transaction amount must be greater than zero",
    "details": [
      {
        "field": "amount",
        "issue": "Value must be positive"
      }
    ],
    "timestamp": "2025-11-13T10:30:00Z",
    "traceId": "abc123-def456"
  }
}
```

### 6.3 Security Error Handling

**Data Integrity Violations**
- Log security alert to audit trail
- Return generic error to user (don't expose details)
- Notify system administrators
- Lock affected records for investigation

**Signature Validation Failures**
- Reject operation immediately
- Log attempted action with user details
- Create audit entry for failed attempt
- Return 403 Forbidden with generic message


## 7. Testing Strategy

### 7.1 Unit Tests

**Domain Layer Tests**
- Entity validation rules
- Value object behavior
- Domain service calculations
- Business rule enforcement

**Application Layer Tests**
- Command handler logic
- Query handler logic
- Service orchestration
- Validation logic

**Infrastructure Layer Tests**
- Cryptographic service operations
- Hash generation and verification
- Signature creation and validation

### 7.2 Integration Tests

**Database Integration**
- Repository operations with real SQL Server
- Entity Framework migrations
- Complex queries and joins
- Transaction rollback scenarios

**API Integration**
- End-to-end request/response flows
- Authentication and authorization
- Error handling and status codes
- Performance validation (<100ms requirement)

### 7.3 Test Data Strategy

**Realistic Test Data**
- Multiple projects with different statuses
- Various transaction types and amounts
- Complete audit trails
- Edge cases (boundary dates, maximum amounts)

**Test Database Setup**
- Spin up SQL Server container
- Apply migrations
- Seed with test data
- Run tests
- Tear down after completion

### 7.4 Security Testing

**Integrity Verification Tests**
- Detect tampered transaction data
- Validate hash computations
- Verify signature validation
- Test audit trail immutability

**Authorization Tests**
- Prevent unauthorized transaction creation
- Validate coordinator-project association
- Test JWT token validation
- Verify role-based access control


## 8. Performance Considerations

### 8.1 Performance Requirements

- All API endpoints must respond in < 100ms
- Database queries optimized with appropriate indexes
- Use AsNoTracking for read-only queries
- Implement pagination for large result sets

### 8.2 Optimization Strategies

**Database Optimization**
- Indexes on frequently queried fields (Status, Date, BankAccountId)
- Compiled queries for repeated operations
- Projection queries to minimize data transfer
- Connection pooling

**Caching Strategy**
- Cache accounting accounts (rarely change)
- Cache project details for balance calculations
- Invalidate cache on updates
- Use distributed cache for scalability

**Query Optimization**
- Use Include() for eager loading related entities
- Avoid N+1 query problems
- Use pagination for transaction history
- Limit audit trail queries with date ranges

### 8.3 Performance Monitoring

- Log response times for all endpoints
- Alert on requests exceeding 100ms threshold
- Monitor database query execution times
- Track cache hit rates

## 9. Security Considerations

### 9.1 Authentication & Authorization

**JWT Token Authentication**
- Tokens issued by authentication service
- Include user ID and roles
- Expire after configurable period
- Refresh token mechanism

**Authorization Rules**
- Only assigned coordinator can create transactions for their project
- Audit trail access restricted to authorized users
- Report generation requires appropriate permissions

### 9.2 Data Protection

**Encryption**
- HTTPS for all API communications
- Encrypt sensitive data at rest (if required)
- Secure storage of cryptographic keys

**Audit Trail Protection**
- Immutable audit entries
- Cryptographic hashes prevent tampering
- Digital signatures ensure non-repudiation
- Regular integrity verification

### 9.3 Security Best Practices

- Input validation on all endpoints
- SQL injection prevention (EF Core parameterized queries)
- CORS configuration for allowed origins
- Rate limiting to prevent abuse
- Security headers (HSTS, CSP, X-Frame-Options)


## 10. Deployment Architecture

### 10.1 Container Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     Docker Compose                       │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  ┌──────────────────┐      ┌──────────────────┐       │
│  │   API Container  │      │  SQL Server      │       │
│  │   (.NET 8)       │─────▶│  Container       │       │
│  │   Port: 5000     │      │  Port: 1433      │       │
│  └──────────────────┘      └──────────────────┘       │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

### 10.2 Environment Configuration

**Development**
- Local SQL Server in Docker
- Swagger UI enabled
- Detailed error messages
- Development JWT secrets

**Production**
- Managed SQL Server instance
- Swagger UI disabled
- Generic error messages
- Secure JWT secrets from key vault
- HTTPS enforced
- Rate limiting enabled

### 10.3 Database Migrations

- Migrations applied on application startup
- Version controlled migration files
- Rollback capability for failed migrations
- Backup before production migrations

## 11. API Documentation

### 11.1 OpenAPI/Swagger Specification

- Complete API documentation with Swagger
- Example requests and responses
- Authentication requirements documented
- Error response schemas
- Performance expectations noted

### 11.2 Documentation Requirements

- Summary and description for each endpoint
- Parameter descriptions and constraints
- Response schemas with examples
- HTTP status codes documented
- Expected response times (<100ms)

## 12. Design Decisions and Rationale

### 12.1 Hexagonal Architecture

**Decision**: Use hexagonal architecture with clear layer separation

**Rationale**:
- Isolates business logic from infrastructure concerns
- Enables testing without external dependencies
- Facilitates technology changes (e.g., switching databases)
- Improves maintainability and code organization

### 12.2 Cryptographic Integrity

**Decision**: Use SHA-256 hashing for data integrity verification

**Rationale**:
- Industry-standard cryptographic algorithm
- Provides strong tamper detection
- Efficient computation for performance requirements
- Widely supported in .NET ecosystem

### 12.3 Immutable Audit Trail

**Decision**: Make audit entries completely immutable

**Rationale**:
- Ensures non-repudiation requirements
- Prevents tampering with historical records
- Provides reliable compliance evidence
- Simplifies integrity verification

### 12.4 Real Database Testing

**Decision**: Use real SQL Server for all tests

**Rationale**:
- Tests actual database constraints and behavior
- Validates performance with realistic data
- Catches issues that in-memory databases miss
- Ensures production-like test environment

## 13. Future Enhancements

- Multi-currency support
- Advanced reporting with charts and analytics
- Bulk transaction import
- Document attachment for fiscal documentation
- Email notifications for budget thresholds
- Role-based access control with multiple user types
- API versioning for backward compatibility
