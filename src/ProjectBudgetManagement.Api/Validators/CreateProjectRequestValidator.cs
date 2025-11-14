using FluentValidation;
using ProjectBudgetManagement.Api.Models;

namespace ProjectBudgetManagement.Api.Validators;

/// <summary>
/// Validator for CreateProjectRequest.
/// </summary>
public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateProjectRequestValidator class.
    /// </summary>
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(200).WithMessage("Project name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Project description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .LessThanOrEqualTo(x => x.EndDate).WithMessage("Start date must be before or equal to end date");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after or equal to start date");

        RuleFor(x => x.BudgetAmount)
            .GreaterThan(0).WithMessage("Budget amount must be greater than zero");

        RuleFor(x => x.CoordinatorId)
            .NotEmpty().WithMessage("Coordinator ID is required");

        RuleFor(x => x.BankAccount)
            .NotNull().WithMessage("Bank account information is required")
            .SetValidator(new BankAccountRequestValidator()!);
    }
}

/// <summary>
/// Validator for BankAccountRequest.
/// </summary>
public class BankAccountRequestValidator : AbstractValidator<BankAccountRequest>
{
    /// <summary>
    /// Initializes a new instance of the BankAccountRequestValidator class.
    /// </summary>
    public BankAccountRequestValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required")
            .Matches(@"^\d+$").WithMessage("Account number must contain only numeric characters")
            .MaximumLength(20).WithMessage("Account number must not exceed 20 characters");

        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage("Bank name is required")
            .MaximumLength(100).WithMessage("Bank name must not exceed 100 characters");

        RuleFor(x => x.BranchNumber)
            .NotEmpty().WithMessage("Branch number is required")
            .Matches(@"^\d+$").WithMessage("Branch number must contain only numeric characters")
            .MaximumLength(10).WithMessage("Branch number must not exceed 10 characters");

        RuleFor(x => x.AccountHolderName)
            .NotEmpty().WithMessage("Account holder name is required")
            .MaximumLength(200).WithMessage("Account holder name must not exceed 200 characters");
    }
}
