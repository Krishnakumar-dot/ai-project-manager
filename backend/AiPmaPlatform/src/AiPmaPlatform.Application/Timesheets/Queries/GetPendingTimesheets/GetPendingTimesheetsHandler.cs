using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Timesheets.Queries.GetPendingTimesheets
{
    public class GetPendingTimesheetsHandler
        : IRequestHandler<GetPendingTimesheetsQuery, ApiResponse<List<PendingTimesheetDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetPendingTimesheetsHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<PendingTimesheetDto>>> Handle(
            GetPendingTimesheetsQuery request, CancellationToken cancellationToken)
        {
            var sheets = await _context.Timesheets
                .AsNoTracking()
                .Where(t => t.Status == TimesheetStatus.Submitted)
                .OrderBy(t => t.SubmittedAt)
                .Select(t => new { t.Id, UserName = t.User!.Name, t.WeekStartDate, t.SubmittedAt })
                .ToListAsync(cancellationToken);

            var ids = sheets.Select(s => s.Id).ToList();

            var entryRows = await _context.TimesheetEntries
                .AsNoTracking()
                .Where(e => ids.Contains(e.TimesheetId))
                .OrderBy(e => e.Date)
                .Select(e => new
                {
                    e.TimesheetId,
                    Dto = new TimesheetEntryDto(
                        e.ProjectId,
                        e.Project!.Name,
                        e.ProjectTaskId,
                        e.ProjectTask != null ? e.ProjectTask.Title : null,
                        e.Date,
                        e.Hours,
                        e.IsBillable,
                        e.Notes)
                })
                .ToListAsync(cancellationToken);

            var bySheet = entryRows.ToLookup(x => x.TimesheetId, x => x.Dto);

            var result = sheets.Select(s =>
            {
                var entries = bySheet[s.Id].ToList();
                return new PendingTimesheetDto(
                    s.Id, s.UserName, s.WeekStartDate, s.SubmittedAt,
                    entries.Sum(e => e.Hours),
                    entries.Where(e => e.IsBillable).Sum(e => e.Hours),
                    entries);
            }).ToList();

            return ApiResponse<List<PendingTimesheetDto>>.Success(result, "Pending timesheets retrieved successfully.");
        }
    }
}