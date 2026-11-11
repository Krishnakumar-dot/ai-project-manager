using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.ResourcePlanning.Queries.GetAllocations
{
    public class GetAllocationsHandler
        : IRequestHandler<GetAllocationsQuery, ApiResponse<List<AllocationDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllocationsHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<AllocationDto>>> Handle(
            GetAllocationsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ResourceAllocations.AsNoTracking().AsQueryable();

            if (request.ProjectId.HasValue)
                query = query.Where(a => a.ProjectId == request.ProjectId.Value);

            var allocations = await query
                .OrderBy(a => a.User!.Name)
                .ThenBy(a => a.StartDate)
                .Select(a => new AllocationDto(
                    a.Id,
                    a.UserId,
                    a.User!.Name,
                    a.ProjectId,
                    a.Project!.Name,
                    a.AllocationPercent,
                    a.StartDate,
                    a.EndDate,
                    a.RoleOnProject))
                .ToListAsync(cancellationToken);

            return ApiResponse<List<AllocationDto>>.Success(allocations, "Allocations retrieved successfully.");
        }
    }
}