using AiPmaPlatform.Domain.Common;
using AiPmaPlatform.Domain.Entities.Identity;
using AiPmaPlatform.Domain.Entities.Portfolio;
using AiPmaPlatform.Domain.Enums;

namespace AiPmaPlatform.Domain.Entities.ProjectTasks
{
    public class ProjectTask : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.ToDo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }

        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }

        public Guid? AssigneeId { get; set; }
        public User? Assignee { get; set; }
    }
}