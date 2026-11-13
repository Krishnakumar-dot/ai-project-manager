using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Raid.Queries.GetRaidSummary
{
    public class GetRaidSummaryHandler : IRequestHandler<GetRaidSummaryQuery, ApiResponse<RaidSummaryDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetRaidSummaryHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<RaidSummaryDto>> Handle(
            GetRaidSummaryQuery request, CancellationToken cancellationToken)
        {
            var query = _context.RaidItems
                .AsNoTracking()
                .Where(i => i.Status == RaidStatus.Open || i.Status == RaidStatus.InProgress);

            if (request.ProjectId.HasValue)
                query = query.Where(i => i.ProjectId == request.ProjectId.Value);

            var active = await query
                .Select(i => new { i.Type, i.Impact, i.Probability, i.DueDate, i.IsEscalated })
                .ToListAsync(cancellationToken);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // "Critical" and "High" only count risks and issues, the things that threaten the project.
            var threatScores = active
                .Where(a => a.Type == RaidItemType.Risk || a.Type == RaidItemType.Issue)
                .Select(a => RaidRules.Score(a.Impact, a.Probability))
                .ToList();

            // The heat map shows active risks only. All 25 cells are returned, including empty ones.
            var risks = active.Where(a => a.Type == RaidItemType.Risk).ToList();
            var heatMap = new List<HeatMapCellDto>();
            for (var impact = 1; impact <= 5; impact++)
            {
                for (var probability = 1; probability <= 5; probability++)
                {
                    heatMap.Add(new HeatMapCellDto(
                        impact,
                        probability,
                        risks.Count(r => r.Impact == impact && r.Probability == probability)));
                }
            }

            var summary = new RaidSummaryDto(
                active.Count,
                active.Count(a => a.Type == RaidItemType.Risk),
                active.Count(a => a.Type == RaidItemType.Assumption),
                active.Count(a => a.Type == RaidItemType.Issue),
                active.Count(a => a.Type == RaidItemType.Dependency),
                threatScores.Count(s => RaidRules.Level(s) == RaidRules.LevelCritical),
                threatScores.Count(s => RaidRules.Level(s) == RaidRules.LevelHigh),
                active.Count(a => a.DueDate.HasValue && a.DueDate.Value < today),
                active.Count(a => a.IsEscalated),
                heatMap);

            return ApiResponse<RaidSummaryDto>.Success(summary, "Summary retrieved successfully.");
        }
    }
}