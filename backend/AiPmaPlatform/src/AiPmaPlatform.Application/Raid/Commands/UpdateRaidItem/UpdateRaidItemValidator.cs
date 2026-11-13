using FluentValidation;

namespace AiPmaPlatform.Application.Raid.Commands.UpdateRaidItem
{
    public class UpdateRaidItemValidator : AbstractValidator<UpdateRaidItemCommand>
    {
        public UpdateRaidItemValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
                                 .MaximumLength(200);

            RuleFor(x => x.Impact).InclusiveBetween(1, 5)
                .WithMessage("Impact must be between 1 and 5.");
            RuleFor(x => x.Probability).InclusiveBetween(1, 5)
                .WithMessage("Probability must be between 1 and 5.");

            RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid status.");

            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.MitigationPlan).MaximumLength(2000);
            RuleFor(x => x.RootCause).MaximumLength(2000);
        }
    }
}