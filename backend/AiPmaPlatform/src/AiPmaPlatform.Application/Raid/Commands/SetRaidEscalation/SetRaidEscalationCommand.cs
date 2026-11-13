using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Raid.Commands.SetRaidEscalation
{
    public class SetRaidEscalationCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
        public bool Escalate { get; set; }
    }
}