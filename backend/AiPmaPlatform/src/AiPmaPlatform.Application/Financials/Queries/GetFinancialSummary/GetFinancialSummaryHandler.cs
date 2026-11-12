using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Financials.Queries.GetFinancialSummary
{
    public class GetFinancialSummaryHandler
        : IRequestHandler<GetFinancialSummaryQuery, ApiResponse<List<ProjectFinancialSummaryDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetFinancialSummaryHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<ProjectFinancialSummaryDto>>> Handle(
            GetFinancialSummaryQuery request, CancellationToken cancellationToken)
        {
            var projects = await _context.Projects
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new ProjectInfo(
                    p.Id, p.Name, p.Portfolio!.Name, p.Budget, p.BillingRatePerHour, p.EndDate))
                .ToListAsync(cancellationToken);

            // Only approved timesheets count towards money.
            var rows = await _context.TimesheetEntries
                .AsNoTracking()
                .Where(e => e.Timesheet!.Status == TimesheetStatus.Approved)
                .Select(e => new ApprovedHoursRow(
                    e.ProjectId,
                    e.Timesheet!.UserId,
                    e.Timesheet!.User!.Name,
                    e.Timesheet!.WeekStartDate,
                    e.Hours,
                    e.IsBillable,
                    e.Timesheet!.User!.HourlyCostRate))
                .ToListAsync(cancellationToken);

            var byProject = rows.ToLookup(r => r.ProjectId);

            var result = projects
                .Select(p => FinancialMath.BuildSummary(p, byProject[p.Id].ToList()))
                .ToList();

            return ApiResponse<List<ProjectFinancialSummaryDto>>.Success(result, "Financial summary retrieved successfully.");
        }
    }
}