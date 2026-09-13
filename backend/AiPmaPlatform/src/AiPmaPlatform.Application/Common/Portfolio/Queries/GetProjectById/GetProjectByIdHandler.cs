using AiPmaPlatform.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Portfolio.Queries.GetProjectById
{
    public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, ProjectDetailDto?>
    {
        private readonly IApplicationDbContext _context;
        public GetProjectByIdHandler(IApplicationDbContext context) => _context = context;

        public async Task<ProjectDetailDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .Include(p => p.Phases)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (project is null) return null;

            return new ProjectDetailDto(
                project.Id, project.Name, project.Description, project.Status.ToString(),
                project.StartDate, project.EndDate,
                project.Phases.Select(ph => new PhaseDto(ph.Id, ph.Name, ph.SequenceOrder)).ToList());
        }
    }
}