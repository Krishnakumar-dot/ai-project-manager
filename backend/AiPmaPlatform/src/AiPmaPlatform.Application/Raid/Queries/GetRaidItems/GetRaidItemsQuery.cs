using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.Raid.Queries.GetRaidItems
{
    public class GetRaidItemsQuery : IRequest<ApiResponse<List<RaidItemDto>>>
    {
        public Guid? ProjectId { get; set; }
        public RaidItemType? Type { get; set; }
        public RaidStatus? Status { get; set; }
        public bool ActiveOnly { get; set; }
    }
}