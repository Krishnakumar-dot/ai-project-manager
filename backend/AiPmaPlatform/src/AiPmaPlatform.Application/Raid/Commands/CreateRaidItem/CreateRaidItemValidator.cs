using FluentValidation;

namespace AiPmaPlatform.Application.Raid.Commands.CreateRaidItem
{
    public class CreateRaidItemValidator : AbstractValidator<CreateRaidItemCommand>
    {
        public CreateRaidItemValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty().WithMessage("Please select a project.");
            RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid item type.");

            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
                                 .MaximumLength(200);

            RuleFor(x => x.Impact).InclusiveBetween(1, 5)
                .WithMessage("Impact must be between 1 and 5.");
            RuleFor(x => x.Probability).InclusiveBetween(1, 5)
                .WithMessage("Probability must be between 1 and 5.");

            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.MitigationPlan).MaximumLength(2000);
            RuleFor(x => x.RootCause).MaximumLength(2000);
        }
    }
}