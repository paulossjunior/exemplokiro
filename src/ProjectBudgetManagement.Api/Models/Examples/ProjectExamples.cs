using Swashbuckle.AspNetCore.Filters;

namespace ProjectBudgetManagement.Api.Models.Examples;

/// <summary>
/// Example for CreateProjectRequest
/// </summary>
public class CreateProjectRequestExample : IExamplesProvider<CreateProjectRequest>
{
    /// <summary>
    /// Provides example data
    /// </summary>
    public CreateProjectRequest GetExamples()
    {
        return new CreateProjectRequest
        {
            Name = "Community Center Renovation",
            Description = "Complete renovation of the downtown community center including new facilities and equipment",
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            BudgetAmount = 150000.00m,
            CoordinatorId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            BankAccount = new BankAccountRequest
            {
                AccountNumber = "12345678",
                BankName = "First National Bank",
                BranchNumber = "001",
                AccountHolderName = "John Smith"
            }
        };
    }
}

/// <summary>
/// Example for ProjectResponse
/// </summary>
public class ProjectResponseExample : IExamplesProvider<ProjectResponse>
{
    /// <summary>
    /// Provides example data
    /// </summary>
    public ProjectResponse GetExamples()
    {
        return new ProjectResponse
        {
            Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Name = "Community Center Renovation",
            Description = "Complete renovation of the downtown community center including new facilities and equipment",
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            Status = "InProgress",
            BudgetAmount = 150000.00m,
            CoordinatorId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
            BankAccount = new BankAccountResponse
            {
                Id = Guid.Parse("9b2e7f3a-4c8d-4e5f-9a1b-2c3d4e5f6a7b"),
                AccountNumber = "12345678",
                BankName = "First National Bank",
                BranchNumber = "001",
                AccountHolderName = "John Smith"
            },
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
    }
}

/// <summary>
/// Example for CreateTransactionRequest
/// </summary>
public class CreateTransactionRequestExample : IExamplesProvider<CreateTransactionRequest>
{
    /// <summary>
    /// Provides example data
    /// </summary>
    public CreateTransactionRequest GetExamples()
    {
        return new CreateTransactionRequest
        {
            Amount = 5000.00m,
            Date = DateTime.UtcNow.Date,
            Classification = "Debit",
            AccountingAccountId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7")
        };
    }
}

/// <summary>
/// Example for TransactionResponse
/// </summary>
public class TransactionResponseExample : IExamplesProvider<TransactionResponse>
{
    /// <summary>
    /// Provides example data
    /// </summary>
    public TransactionResponse GetExamples()
    {
        return new TransactionResponse
        {
            Id = Guid.Parse("8d3f5a2b-6e9c-4f7d-8a1e-3b4c5d6e7f8a"),
            Amount = 5000.00m,
            Date = DateTime.UtcNow.Date,
            Classification = "Debit",
            DigitalSignature = "HMAC-SHA256:a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8s9t0u1v2w3x4y5z6",
            DataHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            BankAccountId = Guid.Parse("9b2e7f3a-4c8d-4e5f-9a1b-2c3d4e5f6a7b"),
            AccountingAccountId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6")
        };
    }
}
