using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Entities.Timesheets;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Timesheets.Commands.SaveTimesheet
{
    public class SaveTimesheetHandler : IRequestHandler<SaveTimesheetCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public SaveTimesheetHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<Guid>> Handle(SaveTimesheetCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (userId is null)
                return ApiResponse<Guid>.Fail("You must be signed in.");

            var weekStart = WeekHelper.StartOfWeek(request.WeekStart);
            var weekEnd = weekStart.AddDays(6);

            if (request.Entries.Any(e => e.Date < weekStart || e.Date > weekEnd))
                return ApiResponse<Guid>.Fail("All entries must fall within the selected week.");

            // Rows with no hours are simply not stored.
            var entries = request.Entries.Where(e => e.Hours > 0).ToList();

            var projectIds = entries.Select(e => e.ProjectId).Distinct().ToList();
            var projectCount = await _context.Projects
                .CountAsync(p => projectIds.Contains(p.Id), cancellationToken);
            if (projectCount != projectIds.Count)
                return ApiResponse<Guid>.Fail("One or more projects were not found.");

            var taskIds = entries.Where(e => e.ProjectTaskId.HasValue)
                                 .Select(e => e.ProjectTaskId!.Value)
                                 .Distinct()
                                 .ToList();
            if (taskIds.Count > 0)
            {
                var tasks = await _context.ProjectTasks
                    .Where(t => taskIds.Contains(t.Id))
                    .Select(t => new { t.Id, t.ProjectId })
                    .ToListAsync(cancellationToken);

                var invalidTask = entries.Any(e =>
                    e.ProjectTaskId.HasValue &&
                    !tasks.Any(t => t.Id == e.ProjectTaskId.Value && t.ProjectId == e.ProjectId));
                if (invalidTask)
                    return ApiResponse<Guid>.Fail("A selected task does not belong to its project.");
            }

            var sheet = await _context.Timesheets
                .Include(t => t.Entries)
                .FirstOrDefaultAsync(t => t.UserId == userId.Value && t.WeekStartDate == weekStart, cancellationToken);

            if (sheet is null)
            {
                sheet = new Timesheet { UserId = userId.Value, WeekStartDate = weekStart };
                _context.Timesheets.Add(sheet);
            }
            else
            {
                if (sheet.Status is TimesheetStatus.Submitted or TimesheetStatus.Approved)
                    return ApiResponse<Guid>.Fail("This timesheet is already submitted and can't be edited.");

                _context.TimesheetEntries.RemoveRange(sheet.Entries);
                sheet.Status = TimesheetStatus.Draft; // editing a rejected sheet makes it a draft again
            }

            foreach (var e in entries)
            {
                _context.TimesheetEntries.Add(new TimesheetEntry
                {
                    TimesheetId = sheet.Id,
                    ProjectId = e.ProjectId,
                    ProjectTaskId = e.ProjectTaskId,
                    Date = e.Date,
                    Hours = e.Hours,
                    IsBillable = e.IsBillable,
                    Notes = e.Notes
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(sheet.Id, "Timesheet saved.");
        }
    }
}