using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Timesheets.Queries.GetPendingTimesheets
{
    public record PendingTimesheetDto(
        Guid Id,
        string UserName,
        DateOnly WeekStartDate,
        DateTime? SubmittedAt,
        decimal TotalHours,
        decimal BillableHours,
        List<TimesheetEntryDto> Entries);

    public class GetPendingTimesheetsQuery : IRequest<ApiResponse<List<PendingTimesheetDto>>> { }
}