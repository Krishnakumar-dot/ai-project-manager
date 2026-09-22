using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Queries.GetPortfoliosList
{
    public record PortfolioListDto(Guid Id, string Name, string? Description, int ProjectCount);

    public class GetPortfoliosListQuery : IRequest<ApiResponse<List<PortfolioListDto>>> { }
}