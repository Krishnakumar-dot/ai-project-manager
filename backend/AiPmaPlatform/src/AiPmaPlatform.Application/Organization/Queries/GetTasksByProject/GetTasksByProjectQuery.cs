using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.ProjectTasks.Queries.GetTasksByProject
{
    public record TaskDto(
        Guid Id,
        string Title,
        string? Description,
        ProjectTaskStatus Status,
        TaskPriority Priority,
        DateTime? DueDate,
        Guid? AssigneeId,
        string? AssigneeName);

    public class GetTasksByProjectQuery : IRequest<ApiResponse<List<TaskDto>>>
    {
        public Guid ProjectId { get; set; }
    }
}