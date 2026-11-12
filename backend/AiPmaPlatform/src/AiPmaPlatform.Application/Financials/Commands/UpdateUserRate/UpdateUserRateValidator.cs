using FluentValidation;

namespace AiPmaPlatform.Application.Financials.Commands.UpdateUserRate
{
    public class UpdateUserRateValidator : AbstractValidator<UpdateUserRateCommand>
    {
        public UpdateUserRateValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();

            RuleFor(x => x.HourlyCostRate)
                .Must(r => r!.Value >= 0m && r.Value <= 100_000m)
                .When(x => x.HourlyCostRate.HasValue)
                .WithMessage("Hourly cost rate must be between 0 and 100,000.");
        }
    }
}