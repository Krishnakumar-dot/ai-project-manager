using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Raid.Queries.GetRaidSummary
{
    public class GetRaidSummaryQuery : IRequest<ApiResponse<RaidSummaryDto>>
    {
        public Guid? ProjectId { get; set; }
    }
}