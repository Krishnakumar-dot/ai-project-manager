using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Financials.Queries.GetProjectFinancials
{
    public class GetProjectFinancialsHandler
        : IRequestHandler<GetProjectFinancialsQuery, ApiResponse<ProjectFinancialDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetProjectFinancialsHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<ProjectFinancialDetailDto>> Handle(
            GetProjectFinancialsQuery request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == request.ProjectId)
                .Select(p => new ProjectInfo(
                    p.Id, p.Name, p.Portfolio!.Name, p.Budget, p.BillingRatePerHour, p.EndDate))
                .FirstOrDefaultAsync(cancellationToken);

            if (project is null)
                return ApiResponse<ProjectFinancialDetailDto>.Fail("Project not found.");

            var rows = await _context.TimesheetEntries
                .AsNoTracking()
                .Where(e => e.ProjectId == request.ProjectId
                         && e.Timesheet!.Status == TimesheetStatus.Approved)
                .Select(e => new ApprovedHoursRow(
                    e.ProjectId,
                    e.Timesheet!.UserId,
                    e.Timesheet!.User!.Name,
                    e.Timesheet!.WeekStartDate,
                    e.Hours,
                    e.IsBillable,
                    e.Timesheet!.User!.HourlyCostRate))
                .ToListAsync(cancellationToken);

            var summary = FinancialMath.BuildSummary(project, rows);

            var byPerson = rows
                .GroupBy(r => new { r.UserId, r.UserName, r.Rate })
                .Select(g => new CostByPersonDto(
                    g.Key.UserId,
                    g.Key.UserName,
                    g.Sum(r => r.Hours),
                    g.Where(r => r.IsBillable).Sum(r => r.Hours),
                    g.Key.Rate,
                    FinancialMath.Round2(g.Sum(r => r.Hours * (r.Rate ?? 0m)))))
                .OrderByDescending(p => p.Cost)
                .ToList();

            var byWeek = rows
                .GroupBy(r => r.WeekStart)
                .OrderBy(g => g.Key)
                .Select(g => new WeeklyCostDto(
                    g.Key,
                    FinancialMath.Round2(g.Sum(r => r.Hours * (r.Rate ?? 0m))),
                    FinancialMath.Round2(project.BillingRate.HasValue
                        ? g.Where(r => r.IsBillable).Sum(r => r.Hours) * project.BillingRate.Value
                        : 0m)))
                .ToList();

            var detail = new ProjectFinancialDetailDto(summary, byPerson, byWeek);
            return ApiResponse<ProjectFinancialDetailDto>.Success(detail, "Project financials retrieved successfully.");
        }
    }
}