using AiPmaPlatform.Domain.Enums;

namespace AiPmaPlatform.Application.Timesheets
{
    public record TimesheetEntryDto(
        Guid ProjectId,
        string ProjectName,
        Guid? ProjectTaskId,
        string? TaskTitle,
        DateOnly Date,
        decimal Hours,
        bool IsBillable,
        string? Notes);

    public record TimesheetDto(
        Guid? Id,
        DateOnly WeekStartDate,
        TimesheetStatus Status,
        string? ReviewComment,
        decimal TotalHours,
        List<TimesheetEntryDto> Entries);
}