using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Raid.Queries.GetRaidItems
{
    public class GetRaidItemsHandler : IRequestHandler<GetRaidItemsQuery, ApiResponse<List<RaidItemDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetRaidItemsHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<RaidItemDto>>> Handle(
            GetRaidItemsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.RaidItems.AsNoTracking().AsQueryable();

            if (request.ProjectId.HasValue)
                query = query.Where(i => i.ProjectId == request.ProjectId.Value);
            if (request.Type.HasValue)
                query = query.Where(i => i.Type == request.Type.Value);
            if (request.Status.HasValue)
                query = query.Where(i => i.Status == request.Status.Value);
            if (request.ActiveOnly)
                query = query.Where(i => i.Status == RaidStatus.Open || i.Status == RaidStatus.InProgress);

            var rows = await query
                .Select(i => new
                {
                    i.Id,
                    i.ProjectId,
                    ProjectName = i.Project!.Name,
                    i.Type,
                    i.Title,
                    i.Description,
                    i.Impact,
                    i.Probability,
                    i.Status,
                    i.OwnerId,
                    OwnerName = i.Owner != null ? i.Owner.Name : null,
                    i.DueDate,
                    i.MitigationPlan,
                    i.RootCause,
                    i.IsEscalated,
                    ReportedByName = i.ReportedBy!.Name,
                    i.CreatedAt,
                    i.ResolvedAt
                })
                .ToListAsync(cancellationToken);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Score, level and overdue are calculated here, in one place, from the stored values.
            var items = rows
                .Select(r =>
                {
                    var score = RaidRules.Score(r.Impact, r.Probability);
                    var overdue = r.DueDate.HasValue && r.DueDate.Value < today && RaidRules.IsActive(r.Status);

                    return new RaidItemDto(
                        r.Id, r.ProjectId, r.ProjectName, r.Type, r.Title, r.Description,
                        r.Impact, r.Probability, score, RaidRules.Level(score), r.Status,
                        r.OwnerId, r.OwnerName, r.DueDate, r.MitigationPlan, r.RootCause,
                        r.IsEscalated, r.ReportedByName, r.CreatedAt, r.ResolvedAt, overdue);
                })
                .OrderByDescending(i => i.IsEscalated)
                .ThenByDescending(i => i.Score)
                .ThenBy(i => i.CreatedAt)
                .ToList();

            return ApiResponse<List<RaidItemDto>>.Success(items, "Items retrieved successfully.");
        }
    }
}