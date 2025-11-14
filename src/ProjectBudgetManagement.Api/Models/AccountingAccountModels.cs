namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// Request DTO for creating an accounting account.
/// </summary>
public class CreateAccountingAccountRequest
{
    /// <summary>
    /// Gets or sets the accounting account name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the accounting account identifier.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for accounting account information.
/// </summary>
public class AccountingAccountResponse
{
    /// <summary>
    /// Gets or sets the accounting account ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the accounting account name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the accounting account identifier.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
