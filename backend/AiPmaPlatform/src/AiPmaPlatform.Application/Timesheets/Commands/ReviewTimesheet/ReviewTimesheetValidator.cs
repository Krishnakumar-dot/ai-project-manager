using FluentValidation;

namespace AiPmaPlatform.Application.Timesheets.Commands.ReviewTimesheet
{
    public class ReviewTimesheetValidator : AbstractValidator<ReviewTimesheetCommand>
    {
        public ReviewTimesheetValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Please add a comment when rejecting a timesheet.")
                .When(x => !x.Approve);

            RuleFor(x => x.Comment).MaximumLength(1000);
        }
    }
}