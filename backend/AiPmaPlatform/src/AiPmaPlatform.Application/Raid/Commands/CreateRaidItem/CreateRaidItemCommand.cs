using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.Raid.Commands.CreateRaidItem
{
    public class CreateRaidItemCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid ProjectId { get; set; }
        public RaidItemType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Impact { get; set; } = 3;
        public int Probability { get; set; } = 3;
        public Guid? OwnerId { get; set; }
        public DateOnly? DueDate { get; set; }
        public string? MitigationPlan { get; set; }
        public string? RootCause { get; set; }
    }
}