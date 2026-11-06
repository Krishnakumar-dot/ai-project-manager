using AiPmaPlatform.Domain.Common;
using AiPmaPlatform.Domain.Entities.Portfolio;
using AiPmaPlatform.Domain.Entities.ProjectTasks;

namespace AiPmaPlatform.Domain.Entities.Timesheets
{
    public class TimesheetEntry : BaseEntity
    {
        public Guid TimesheetId { get; set; }
        public Timesheet? Timesheet { get; set; }

        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }

        public Guid? ProjectTaskId { get; set; }
        public ProjectTask? ProjectTask { get; set; }

        public DateOnly Date { get; set; }
        public decimal Hours { get; set; }
        public bool IsBillable { get; set; } = true;
        public string? Notes { get; set; }
    }
}