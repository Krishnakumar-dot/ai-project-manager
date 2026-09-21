using FluentValidation;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreateMilestone
{
    public class CreateMilestoneValidator : AbstractValidator<CreateMilestoneCommand>
    {
        public CreateMilestoneValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.PhaseId).NotEmpty();
            RuleFor(x => x.DueDate).NotEmpty();
        }
    }
}