using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.ResourcePlanning.Queries.GetUtilization
{
    public class GetUtilizationQuery : IRequest<ApiResponse<List<UtilizationDto>>>
    {
        public DateOnly WeekStart { get; set; }
    }
}