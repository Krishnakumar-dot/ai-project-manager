using FluentValidation;

namespace AiPmaPlatform.Application.ProjectTasks.Commands.CreateTask
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Task title is required.")
                                 .MaximumLength(200);
            RuleFor(x => x.ProjectId).NotEmpty().WithMessage("Project is required.");
            RuleFor(x => x.Priority).IsInEnum();
        }
    }
}