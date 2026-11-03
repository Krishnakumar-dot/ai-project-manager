using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.ProjectTasks.Commands.CreateTask
{
    public class CreateTaskCommand : IRequest<ApiResponse<Guid>>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
        public Guid? AssigneeId { get; set; }
    }
}