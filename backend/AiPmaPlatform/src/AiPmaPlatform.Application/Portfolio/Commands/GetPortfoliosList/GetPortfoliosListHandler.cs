using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Portfolio.Queries.GetPortfoliosList
{
    public class GetPortfoliosListHandler : IRequestHandler<GetPortfoliosListQuery, ApiResponse<List<PortfolioListDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetPortfoliosListHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<PortfolioListDto>>> Handle(GetPortfoliosListQuery request, CancellationToken cancellationToken)
        {
            var portfolios = await _context.Portfolios
                .Select(p => new PortfolioListDto(p.Id, p.Name, p.Description, p.Projects.Count))
                .ToListAsync(cancellationToken);

            return ApiResponse<List<PortfolioListDto>>.Success(portfolios, "Portfolios retrieved successfully.");
        }
    }
}