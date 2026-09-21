using FluentValidation;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreatePhase
{
    public class CreatePhaseValidator : AbstractValidator<CreatePhaseCommand>
    {
        public CreatePhaseValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.SequenceOrder).GreaterThanOrEqualTo(0);
        }
    }
}