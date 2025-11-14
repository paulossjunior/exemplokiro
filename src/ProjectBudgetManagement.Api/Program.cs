using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Project Budget Management API",
        Version = "v1",
        Description = "API for managing project budgets with complete audit trails and non-repudiation"
    });
});

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

// TODO: Configure Entity Framework Core with SQL Server
// builder.Services.AddDbContext<ProjectBudgetDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// TODO: Register repositories (Infrastructure layer)
// builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
// builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
// builder.Services.AddScoped<IAccountingAccountRepository, AccountingAccountRepository>();
// builder.Services.AddScoped<IAuditRepository, AuditRepository>();
// builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// TODO: Register domain services
// builder.Services.AddScoped<BalanceCalculationService>();
// builder.Services.AddScoped<CryptographicService>();
// builder.Services.AddScoped<DigitalSignatureService>();
// builder.Services.AddScoped<IntegrityVerificationService>();

// TODO: Register application services
// builder.Services.AddScoped<ProjectService>();
// builder.Services.AddScoped<TransactionService>();
// builder.Services.AddScoped<AuditService>();
// builder.Services.AddScoped<ReportingService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Budget Management API v1");
        options.RoutePrefix = "swagger";
    });
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    await next();
});

app.UseHttpsRedirection();
app.UseCors();

// TODO: Add authentication middleware
// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
