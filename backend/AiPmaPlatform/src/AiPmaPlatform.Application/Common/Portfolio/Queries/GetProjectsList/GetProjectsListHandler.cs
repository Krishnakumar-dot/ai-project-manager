using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Portfolio.Queries.GetProjectsList
{
    public class GetProjectsListHandler : IRequestHandler<GetProjectsListQuery, ApiResponse<List<ProjectListDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetProjectsListHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<ProjectListDto>>> Handle(GetProjectsListQuery request, CancellationToken cancellationToken)
        {
            var projects = await _context.Projects
                .Include(p => p.Portfolio)
                .Select(p => new ProjectListDto(p.Id, p.Name, p.Status, p.StartDate, p.EndDate, p.Portfolio!.Name))
                .ToListAsync(cancellationToken);

            return ApiResponse<List<ProjectListDto>>.Success(projects, "Projects retrieved successfully.");
        }
    }
}