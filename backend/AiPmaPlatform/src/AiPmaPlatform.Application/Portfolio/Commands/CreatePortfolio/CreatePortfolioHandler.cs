using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using PortfolioEntity = AiPmaPlatform.Domain.Entities.Portfolio.Portfolio;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreatePortfolio
{
    public class CreatePortfolioHandler : IRequestHandler<CreatePortfolioCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public CreatePortfolioHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<Guid>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var portfolio = new PortfolioEntity
            {
                Name = request.Name,
                Description = request.Description
            };

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(portfolio.Id, "Portfolio created successfully.");
        }
    }
}