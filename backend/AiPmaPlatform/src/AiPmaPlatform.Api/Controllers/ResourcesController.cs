using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.ResourcePlanning;
using AiPmaPlatform.Application.ResourcePlanning.Commands.CreateAllocation;
using AiPmaPlatform.Application.ResourcePlanning.Commands.DeleteAllocation;
using AiPmaPlatform.Application.ResourcePlanning.Queries.GetAllocations;
using AiPmaPlatform.Application.ResourcePlanning.Queries.GetUtilization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,PM,PMO")]
    public class ResourcesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ResourcesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("allocations")]
        public async Task<ActionResult<ApiResponse<List<AllocationDto>>>> GetAllocations([FromQuery] Guid? projectId)
            => Ok(await _mediator.Send(new GetAllocationsQuery { ProjectId = projectId }));

        [HttpPost("allocations")]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateAllocation(CreateAllocationCommand command)
            => Ok(await _mediator.Send(command));

        [HttpDelete("allocations/{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> DeleteAllocation(Guid id)
            => Ok(await _mediator.Send(new DeleteAllocationCommand { Id = id }));

        [HttpGet("utilization")]
        public async Task<ActionResult<ApiResponse<List<UtilizationDto>>>> GetUtilization([FromQuery] DateOnly weekStart)
            => Ok(await _mediator.Send(new GetUtilizationQuery { WeekStart = weekStart }));
    }
}