using AiPmaPlatform.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Portfolio.Queries.GetProjectsList
{
    public class GetProjectsListHandler : IRequestHandler<GetProjectsListQuery, List<ProjectListDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetProjectsListHandler(IApplicationDbContext context) => _context = context;

        public async Task<List<ProjectListDto>> Handle(GetProjectsListQuery request, CancellationToken cancellationToken)
        {
            return await _context.Projects
                .Include(p => p.Portfolio)
                .Select(p => new ProjectListDto(p.Id, p.Name, p.Status, p.StartDate, p.EndDate, p.Portfolio!.Name))
                .ToListAsync(cancellationToken);
        }
    }
}