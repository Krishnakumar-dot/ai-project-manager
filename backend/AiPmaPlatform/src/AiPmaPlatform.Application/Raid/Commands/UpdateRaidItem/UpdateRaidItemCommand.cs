using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.Raid.Commands.UpdateRaidItem
{
    // The type and the project of an item never change. Log a new item instead.
    public class UpdateRaidItemCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Impact { get; set; }
        public int Probability { get; set; }
        public RaidStatus Status { get; set; }
        public Guid? OwnerId { get; set; }
        public DateOnly? DueDate { get; set; }
        public string? MitigationPlan { get; set; }
        public string? RootCause { get; set; }
    }
}