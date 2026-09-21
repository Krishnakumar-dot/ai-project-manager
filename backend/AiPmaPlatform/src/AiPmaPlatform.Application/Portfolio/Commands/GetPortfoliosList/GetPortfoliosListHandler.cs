using AiPmaPlatform.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Portfolio.Queries.GetPortfoliosList
{
    public class GetPortfoliosListHandler : IRequestHandler<GetPortfoliosListQuery, List<PortfolioListDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetPortfoliosListHandler(IApplicationDbContext context) => _context = context;

        public async Task<List<PortfolioListDto>> Handle(GetPortfoliosListQuery request, CancellationToken cancellationToken)
        {
            return await _context.Portfolios
                .Select(p => new PortfolioListDto(p.Id, p.Name, p.Description, p.Projects.Count))
                .ToListAsync(cancellationToken);
        }
    }
}