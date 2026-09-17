using FluentValidation;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreatePortfolio
{
    public class CreatePortfolioValidator : AbstractValidator<CreatePortfolioCommand>
    {
        public CreatePortfolioValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}