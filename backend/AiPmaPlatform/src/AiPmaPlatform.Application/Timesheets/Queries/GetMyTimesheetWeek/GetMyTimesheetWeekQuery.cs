using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Timesheets.Queries.GetMyTimesheetWeek
{
    public class GetMyTimesheetWeekQuery : IRequest<ApiResponse<TimesheetDto>>
    {
        public DateOnly WeekStart { get; set; }
    }
}