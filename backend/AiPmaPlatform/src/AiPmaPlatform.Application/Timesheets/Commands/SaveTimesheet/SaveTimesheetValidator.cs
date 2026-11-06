using FluentValidation;

namespace AiPmaPlatform.Application.Timesheets.Commands.SaveTimesheet
{
    public class SaveTimesheetValidator : AbstractValidator<SaveTimesheetCommand>
    {
        public SaveTimesheetValidator()
        {
            RuleFor(x => x.WeekStart)
                .NotEqual(default(DateOnly)).WithMessage("Week start date is required.");

            RuleForEach(x => x.Entries).ChildRules(entry =>
            {
                entry.RuleFor(e => e.ProjectId).NotEmpty().WithMessage("Each entry needs a project.");
                entry.RuleFor(e => e.Hours).InclusiveBetween(0m, 24m)
                     .WithMessage("Hours per entry must be between 0 and 24.");
                entry.RuleFor(e => e.Notes).MaximumLength(500);
            });

            RuleFor(x => x.Entries)
                .Must(entries => entries.GroupBy(e => e.Date).All(g => g.Sum(e => e.Hours) <= 24))
                .WithMessage("You can't log more than 24 hours in a single day.");
        }
    }
}