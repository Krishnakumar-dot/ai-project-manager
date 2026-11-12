using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Financials.Queries.GetFinancialSummary
{
    public class GetFinancialSummaryQuery : IRequest<ApiResponse<List<ProjectFinancialSummaryDto>>> { }
}