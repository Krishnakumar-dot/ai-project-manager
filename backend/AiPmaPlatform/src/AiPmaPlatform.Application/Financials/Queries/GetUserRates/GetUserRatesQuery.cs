using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Financials.Queries.GetUserRates
{
    public class GetUserRatesQuery : IRequest<ApiResponse<List<UserRateDto>>> { }
}