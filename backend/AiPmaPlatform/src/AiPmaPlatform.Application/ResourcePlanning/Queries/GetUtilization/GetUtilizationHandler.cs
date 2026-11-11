using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.Timesheets;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.ResourcePlanning.Queries.GetUtilization
{
    public class GetUtilizationHandler
        : IRequestHandler<GetUtilizationQuery, ApiResponse<List<UtilizationDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetUtilizationHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<UtilizationDto>>> Handle(
            GetUtilizationQuery request, CancellationToken cancellationToken)
        {
            var weekStart = WeekHelper.StartOfWeek(request.WeekStart);
            var weekEnd = weekStart.AddDays(6);

            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .Select(u => new { u.Id, u.Name })
                .ToListAsync(cancellationToken);

            var allocations = await _context.ResourceAllocations
                .AsNoTracking()
                .Where(a => a.StartDate <= weekEnd && a.EndDate >= weekStart)
                .Select(a => new { a.UserId, a.AllocationPercent, ProjectName = a.Project!.Name })
                .ToListAsync(cancellationToken);

            // Only submitted or approved hours count as "logged". Drafts are still in progress.
            var loggedRows = await _context.TimesheetEntries
                .AsNoTracking()
                .Where(e => e.Date >= weekStart && e.Date <= weekEnd
                         && (e.Timesheet!.Status == TimesheetStatus.Submitted
                             || e.Timesheet!.Status == TimesheetStatus.Approved))
                .Select(e => new { e.Timesheet!.UserId, e.Hours })
                .ToListAsync(cancellationToken);

            var result = users.Select(u =>
            {
                var userAllocations = allocations.Where(a => a.UserId == u.Id).ToList();
                var allocatedPercent = userAllocations.Sum(a => a.AllocationPercent);
                var logged = loggedRows.Where(r => r.UserId == u.Id).Sum(r => r.Hours);

                var status = allocatedPercent == 0
                    ? ResourcePlanningRules.StatusBench
                    : allocatedPercent > 100
                        ? ResourcePlanningRules.StatusOverallocated
                        : ResourcePlanningRules.StatusAllocated;

                return new UtilizationDto(
                    u.Id,
                    u.Name,
                    allocatedPercent,
                    ResourcePlanningRules.WeeklyCapacityHours,
                    ResourcePlanningRules.WeeklyCapacityHours * allocatedPercent / 100m,
                    logged,
                    status,
                    userAllocations.Select(a => a.ProjectName).Distinct().ToList());
            }).ToList();

            return ApiResponse<List<UtilizationDto>>.Success(result, "Utilization retrieved successfully.");
        }
    }
}