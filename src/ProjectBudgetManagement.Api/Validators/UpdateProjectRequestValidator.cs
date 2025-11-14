using FluentValidation;
using ProjectBudgetManagement.Api.Models;

namespace ProjectBudgetManagement.Api.Validators;

/// <summary>
/// Validator for UpdateProjectRequest.
/// </summary>
public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateProjectRequestValidator class.
    /// </summary>
    public UpdateProjectRequestValidator()
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
    }
}
