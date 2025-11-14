using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProjectBudgetManagement.Api.Filters;
using ProjectBudgetManagement.Application.Commands;
using ProjectBudgetManagement.Application.Ports;
using ProjectBudgetManagement.Application.Queries;
using ProjectBudgetManagement.Application.Services;
using ProjectBudgetManagement.Domain.Services;
using ProjectBudgetManagement.Infrastructure.Persistence;
using ProjectBudgetManagement.Infrastructure.Repositories;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Project Budget Management API",
        Version = "v1",
        Description = @"
# Project Budget Management API

A comprehensive financial accountability platform that enables project coordinators to manage projects with dedicated bank accounts and track financial transactions with complete audit trails and non-repudiation guarantees.

## Key Features

- **Project Management**: Create and manage projects with budgets and dedicated bank accounts
- **Transaction Tracking**: Record financial transactions with digital signatures
- **Audit Trail**: Complete, immutable audit trail for all system operations
- **Non-Repudiation**: Cryptographic signatures ensure transaction authenticity
- **Data Integrity**: SHA-256 hashing detects any data tampering
- **Accountability Reports**: Generate comprehensive reports with supporting evidence

## Performance

All API endpoints are designed to respond in **less than 100ms** under normal load conditions.

## Authentication

Most endpoints require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Error Handling

The API uses standard HTTP status codes:
- **200 OK**: Successful request
- **201 Created**: Resource successfully created
- **400 Bad Request**: Invalid input or business rule violation
- **401 Unauthorized**: Missing or invalid authentication
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **409 Conflict**: Duplicate resource or constraint violation
- **500 Internal Server Error**: Server error or data integrity issue
",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Project Budget Management Team",
            Email = "support@projectbudget.example.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Enable annotations for additional metadata
    options.EnableAnnotations();

    // Enable example filters for request/response examples
    options.ExampleFilters();
});

// Register example providers
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ProjectBudgetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure memory cache for performance optimization
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ProjectBudgetManagement.Infrastructure.Services.CachingService>();

// Configure performance monitoring
builder.Services.AddSingleton<ProjectBudgetManagement.Api.Services.PerformanceMetricsService>();

// Register repositories (Infrastructure layer) - Scoped lifetime
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IAccountingAccountRepository, AccountingAccountRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// Register domain services - Scoped lifetime
builder.Services.AddScoped<BalanceCalculationService>();
builder.Services.AddScoped<CryptographicService>();
builder.Services.AddScoped<IntegrityVerificationService>();

// Register DigitalSignatureService with configuration - Singleton lifetime
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-secret-key-min-32-chars-long-for-security";
builder.Services.AddSingleton(sp => new DigitalSignatureService(jwtSecret));

// Register application services - Scoped lifetime
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<ReportingService>();
builder.Services.AddScoped<AuthenticationService>(sp =>
{
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProjectBudgetManagement";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ProjectBudgetManagement";
    return new AuthenticationService(jwtSecret, jwtIssuer, jwtAudience);
});
builder.Services.AddScoped<AuthorizationService>();

// Register command handlers - Scoped lifetime
builder.Services.AddScoped<CreateProjectCommandHandler>();
builder.Services.AddScoped<UpdateProjectCommandHandler>();
builder.Services.AddScoped<UpdateProjectStatusCommandHandler>();
builder.Services.AddScoped<CreateTransactionCommandHandler>();
builder.Services.AddScoped<CreateAccountingAccountCommandHandler>();
builder.Services.AddScoped<GenerateAccountabilityReportCommandHandler>();

// Register query handlers - Scoped lifetime
builder.Services.AddScoped<GetProjectQueryHandler>();
builder.Services.AddScoped<ListProjectsQueryHandler>();
builder.Services.AddScoped<GetTransactionHistoryQueryHandler>();
builder.Services.AddScoped<GetAccountBalanceQueryHandler>();
builder.Services.AddScoped<ListAccountingAccountsQueryHandler>();
builder.Services.AddScoped<GetAuditTrailQueryHandler>();
builder.Services.AddScoped<VerifyDataIntegrityQueryHandler>();

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline

// Performance monitoring middleware (should be early in pipeline)
app.UseMiddleware<ProjectBudgetManagement.Api.Middleware.PerformanceMonitoringMiddleware>();

// Global exception handling middleware
app.UseMiddleware<ProjectBudgetManagement.Api.Middleware.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Budget Management API v1");
        options.RoutePrefix = "swagger";
    });
}

// Security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'";
    await next();
});

app.UseHttpsRedirection();
app.UseCors();

// Authentication and authorization middleware
// TODO: Configure JWT authentication when ready
// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class accessible to integration tests
public partial class Program { }
