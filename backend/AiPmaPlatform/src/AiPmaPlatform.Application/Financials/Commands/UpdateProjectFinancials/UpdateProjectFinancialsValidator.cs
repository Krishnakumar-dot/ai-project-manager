using FluentValidation;

namespace AiPmaPlatform.Application.Financials.Commands.UpdateProjectFinancials
{
    public class UpdateProjectFinancialsValidator : AbstractValidator<UpdateProjectFinancialsCommand>
    {
        public UpdateProjectFinancialsValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();

            RuleFor(x => x.Budget)
                .Must(b => b!.Value >= 0m && b.Value <= 1_000_000_000m)
                .When(x => x.Budget.HasValue)
                .WithMessage("Budget must be between 0 and 1,000,000,000.");

            RuleFor(x => x.BillingRatePerHour)
                .Must(r => r!.Value >= 0m && r.Value <= 100_000m)
                .When(x => x.BillingRatePerHour.HasValue)
                .WithMessage("Billing rate must be between 0 and 100,000 per hour.");
        }
    }
}