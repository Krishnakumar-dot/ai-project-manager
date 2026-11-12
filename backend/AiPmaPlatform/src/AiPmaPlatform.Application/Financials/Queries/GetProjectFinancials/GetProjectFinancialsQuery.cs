using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Financials.Queries.GetProjectFinancials
{
    public class GetProjectFinancialsQuery : IRequest<ApiResponse<ProjectFinancialDetailDto>>
    {
        public Guid ProjectId { get; set; }
    }
}