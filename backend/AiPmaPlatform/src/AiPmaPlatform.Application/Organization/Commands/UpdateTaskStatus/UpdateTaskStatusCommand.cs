using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.ProjectTasks.Commands.UpdateTaskStatus
{
    public class UpdateTaskStatusCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
        public ProjectTaskStatus Status { get; set; }
    }
}