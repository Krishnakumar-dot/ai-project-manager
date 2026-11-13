using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.Raid;
using AiPmaPlatform.Application.Raid.Commands.CreateRaidItem;
using AiPmaPlatform.Application.Raid.Commands.DeleteRaidItem;
using AiPmaPlatform.Application.Raid.Commands.SetRaidEscalation;
using AiPmaPlatform.Application.Raid.Commands.UpdateRaidItem;
using AiPmaPlatform.Application.Raid.Queries.GetRaidItems;
using AiPmaPlatform.Application.Raid.Queries.GetRaidSummary;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RaidController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RaidController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RaidItemDto>>>> GetAll(
            [FromQuery] Guid? projectId,
            [FromQuery] RaidItemType? type,
            [FromQuery] RaidStatus? status,
            [FromQuery] bool activeOnly = false)
            => Ok(await _mediator.Send(new GetRaidItemsQuery
            {
                ProjectId = projectId,
                Type = type,
                Status = status,
                ActiveOnly = activeOnly
            }));

        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<RaidSummaryDto>>> GetSummary([FromQuery] Guid? projectId)
            => Ok(await _mediator.Send(new GetRaidSummaryQuery { ProjectId = projectId }));

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> Create(CreateRaidItemCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> Update(Guid id, UpdateRaidItemCommand command)
        {
            command.Id = id; // the route value wins over the body
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}/escalation")]
        public async Task<ActionResult<ApiResponse<Guid>>> SetEscalation(Guid id, SetRaidEscalationCommand command)
        {
            command.Id = id;
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,PMO,PM")]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
            => Ok(await _mediator.Send(new DeleteRaidItemCommand { Id = id }));
    }
}