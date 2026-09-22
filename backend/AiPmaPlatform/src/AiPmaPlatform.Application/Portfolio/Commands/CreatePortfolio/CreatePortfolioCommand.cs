using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreatePortfolio
{
    public class CreatePortfolioCommand : IRequest<ApiResponse<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}