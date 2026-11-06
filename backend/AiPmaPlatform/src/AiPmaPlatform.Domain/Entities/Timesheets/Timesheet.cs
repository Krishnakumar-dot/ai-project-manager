using AiPmaPlatform.Domain.Common;
using AiPmaPlatform.Domain.Entities.Identity;
using AiPmaPlatform.Domain.Enums;

namespace AiPmaPlatform.Domain.Entities.Timesheets
{
    public class Timesheet : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        // Always the Monday of the week. DateOnly avoids timezone shifts.
        public DateOnly WeekStartDate { get; set; }

        public TimesheetStatus Status { get; set; } = TimesheetStatus.Draft;
        public DateTime? SubmittedAt { get; set; }

        public Guid? ReviewedById { get; set; }
        public User? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewComment { get; set; }

        public ICollection<TimesheetEntry> Entries { get; set; } = new List<TimesheetEntry>();
    }
}