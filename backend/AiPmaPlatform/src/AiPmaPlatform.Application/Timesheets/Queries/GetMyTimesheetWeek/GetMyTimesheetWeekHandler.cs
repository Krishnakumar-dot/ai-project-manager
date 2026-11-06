using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Timesheets.Queries.GetMyTimesheetWeek
{
    public class GetMyTimesheetWeekHandler
        : IRequestHandler<GetMyTimesheetWeekQuery, ApiResponse<TimesheetDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetMyTimesheetWeekHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<TimesheetDto>> Handle(
            GetMyTimesheetWeekQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (userId is null)
                return ApiResponse<TimesheetDto>.Fail("You must be signed in.");

            var weekStart = WeekHelper.StartOfWeek(request.WeekStart);

            var sheet = await _context.Timesheets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserId == userId.Value && t.WeekStartDate == weekStart, cancellationToken);

            // No sheet yet: return an empty draft. Nothing is saved until the user saves.
            if (sheet is null)
            {
                var empty = new TimesheetDto(null, weekStart, TimesheetStatus.Draft, null, 0, new List<TimesheetEntryDto>());
                return ApiResponse<TimesheetDto>.Success(empty, "No timesheet for this week yet.");
            }

            var entries = await _context.TimesheetEntries
                .AsNoTracking()
                .Where(e => e.TimesheetId == sheet.Id)
                .OrderBy(e => e.Date)
                .Select(e => new TimesheetEntryDto(
                    e.ProjectId,
                    e.Project!.Name,
                    e.ProjectTaskId,
                    e.ProjectTask != null ? e.ProjectTask.Title : null,
                    e.Date,
                    e.Hours,
                    e.IsBillable,
                    e.Notes))
                .ToListAsync(cancellationToken);

            var dto = new TimesheetDto(
                sheet.Id, sheet.WeekStartDate, sheet.Status, sheet.ReviewComment,
                entries.Sum(e => e.Hours), entries);

            return ApiResponse<TimesheetDto>.Success(dto, "Timesheet retrieved successfully.");
        }
    }
}