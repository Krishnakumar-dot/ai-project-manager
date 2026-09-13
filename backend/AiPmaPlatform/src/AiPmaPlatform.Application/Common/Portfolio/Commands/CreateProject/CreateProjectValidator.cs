using FluentValidation;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreateProject
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.PortfolioId).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
        }
    }
}