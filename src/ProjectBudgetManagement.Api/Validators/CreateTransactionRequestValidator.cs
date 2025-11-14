using FluentValidation;
using ProjectBudgetManagement.Api.Models;

namespace ProjectBudgetManagement.Api.Validators;

/// <summary>
/// Validator for CreateTransactionRequest.
/// </summary>
public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateTransactionRequestValidator class.
    /// </summary>
    public CreateTransactionRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transaction amount must be greater than zero");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Transaction date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Transaction date cannot be in the future");

        RuleFor(x => x.Classification)
            .NotEmpty().WithMessage("Transaction classification is required")
            .Must(c => c.Equals("Debit", StringComparison.OrdinalIgnoreCase) || 
                       c.Equals("Credit", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Transaction classification must be either 'Debit' or 'Credit'");

        RuleFor(x => x.AccountingAccountId)
            .NotEmpty().WithMessage("Accounting account ID is required");
    }
}
