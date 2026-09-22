using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreateMilestone
{
    public class CreateMilestoneCommand : IRequest<ApiResponse<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public Guid PhaseId { get; set; }
        public DateTime DueDate { get; set; }
    }
}