using FluentValidation;
using ProjectBudgetManagement.Api.Models;

namespace ProjectBudgetManagement.Api.Validators;

/// <summary>
/// Validator for CreateAccountingAccountRequest.
/// </summary>
public class CreateAccountingAccountRequestValidator : AbstractValidator<CreateAccountingAccountRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateAccountingAccountRequestValidator class.
    /// </summary>
    public CreateAccountingAccountRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Accounting account name is required")
            .MaximumLength(200).WithMessage("Accounting account name must not exceed 200 characters");

        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage("Accounting account identifier is required")
            .MaximumLength(50).WithMessage("Accounting account identifier must not exceed 50 characters")
            .Matches(@"^[A-Z0-9]+(\.[A-Z0-9]+)*$").WithMessage("Accounting account identifier must follow the pattern (e.g., 'XXXX.XX.XXXX')");
    }
}
