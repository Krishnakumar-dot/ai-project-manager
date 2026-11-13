using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Raid.Commands.DeleteRaidItem
{
    public class DeleteRaidItemCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
    }
}