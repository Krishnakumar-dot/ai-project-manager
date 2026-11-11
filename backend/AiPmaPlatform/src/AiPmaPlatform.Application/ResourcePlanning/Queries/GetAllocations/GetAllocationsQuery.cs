using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.ResourcePlanning.Queries.GetAllocations
{
    public class GetAllocationsQuery : IRequest<ApiResponse<List<AllocationDto>>>
    {
        public Guid? ProjectId { get; set; }
    }
}