using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.ResourcePlanning.Commands.DeleteAllocation
{
    public class DeleteAllocationCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
    }
}