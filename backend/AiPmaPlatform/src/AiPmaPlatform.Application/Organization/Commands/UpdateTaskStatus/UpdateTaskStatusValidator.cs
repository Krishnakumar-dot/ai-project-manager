using FluentValidation;

namespace AiPmaPlatform.Application.ProjectTasks.Commands.UpdateTaskStatus
{
    public class UpdateTaskStatusValidator : AbstractValidator<UpdateTaskStatusCommand>
    {
        public UpdateTaskStatusValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}