namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Command to create a new accounting account.
/// </summary>
public class CreateAccountingAccountCommand
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
