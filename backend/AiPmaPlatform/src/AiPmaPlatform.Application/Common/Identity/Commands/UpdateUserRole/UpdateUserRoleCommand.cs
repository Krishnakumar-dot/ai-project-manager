using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Identity.Commands.UpdateUserRole
{
    public class UpdateUserRoleCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}