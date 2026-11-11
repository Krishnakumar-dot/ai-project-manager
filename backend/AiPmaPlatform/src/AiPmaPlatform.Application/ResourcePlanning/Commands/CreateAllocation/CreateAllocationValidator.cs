using FluentValidation;

namespace AiPmaPlatform.Application.ResourcePlanning.Commands.CreateAllocation
{
    public class CreateAllocationValidator : AbstractValidator<CreateAllocationCommand>
    {
        public CreateAllocationValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Please select a person.");
            RuleFor(x => x.ProjectId).NotEmpty().WithMessage("Please select a project.");

            RuleFor(x => x.AllocationPercent)
                .InclusiveBetween(1, 100).WithMessage("Allocation must be between 1% and 100%.");

            RuleFor(x => x.StartDate)
                .NotEqual(default(DateOnly)).WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEqual(default(DateOnly)).WithMessage("End date is required.")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("End date must be on or after the start date.");

            // Also keeps the week-by-week check above to a sensible number of loops.
            RuleFor(x => x)
                .Must(x => x.EndDate <= x.StartDate.AddYears(3))
                .WithMessage("An allocation can span at most 3 years.");

            RuleFor(x => x.RoleOnProject).MaximumLength(100);
        }
    }
}