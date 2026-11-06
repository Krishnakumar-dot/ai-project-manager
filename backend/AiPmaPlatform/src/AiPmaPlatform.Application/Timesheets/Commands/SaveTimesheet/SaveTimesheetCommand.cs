using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Timesheets.Commands.SaveTimesheet
{
    public class TimesheetEntryInput
    {
        public Guid ProjectId { get; set; }
        public Guid? ProjectTaskId { get; set; }
        public DateOnly Date { get; set; }
        public decimal Hours { get; set; }
        public bool IsBillable { get; set; } = true;
        public string? Notes { get; set; }
    }

    public class SaveTimesheetCommand : IRequest<ApiResponse<Guid>>
    {
        public DateOnly WeekStart { get; set; }
        public List<TimesheetEntryInput> Entries { get; set; } = new();
    }
}