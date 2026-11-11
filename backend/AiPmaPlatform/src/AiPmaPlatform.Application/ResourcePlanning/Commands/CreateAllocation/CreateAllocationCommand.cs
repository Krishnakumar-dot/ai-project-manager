using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.ResourcePlanning.Commands.CreateAllocation
{
    public class CreateAllocationCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
        public int AllocationPercent { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? RoleOnProject { get; set; }

        // Lets a manager knowingly go past 100%, for example to cover a short crunch.
        public bool AllowOverallocation { get; set; }
    }
}