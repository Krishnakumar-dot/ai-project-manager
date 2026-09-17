using AiPmaPlatform.Application.Common.Interfaces;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreatePortfolio
{
    public class CreatePortfolioHandler : IRequestHandler<CreatePortfolioCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        public CreatePortfolioHandler(IApplicationDbContext context) => _context = context;

        public async Task<Guid> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var portfolio = new Domain.Entities.Portfolio.Portfolio
            {
                Name = request.Name,
                Description = request.Description
            };

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync(cancellationToken);
            return portfolio.Id;
        }
    }
}