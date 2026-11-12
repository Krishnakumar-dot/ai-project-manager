using AiPmaPlatform.Application.Common.Constants;
using FluentValidation;

namespace AiPmaPlatform.Application.Identity.Commands.UpdateUserRole
{
    public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        public UpdateUserRoleValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Role)
                .Must(role => AppRoles.All.Contains(role))
                .WithMessage($"Role must be one of: {string.Join(", ", AppRoles.All)}.");
        }
    }
}