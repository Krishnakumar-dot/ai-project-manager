using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.Portfolio.Commands.CreateMilestone;
using AiPmaPlatform.Application.Portfolio.Commands.CreatePhase;
using AiPmaPlatform.Application.Portfolio.Commands.CreatePortfolio;
using AiPmaPlatform.Application.Portfolio.Queries.GetPortfoliosList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PortfolioController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PortfolioController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> Create(CreatePortfolioCommand command)
    => Ok(await _mediator.Send(command));

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PortfolioListDto>>>> GetAll()
            => Ok(await _mediator.Send(new GetPortfoliosListQuery()));

        [HttpPost("phases")]
        public async Task<ActionResult<ApiResponse<Guid>>> CreatePhase(CreatePhaseCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPost("milestones")]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateMilestone(CreateMilestoneCommand command)
            => Ok(await _mediator.Send(command));
    }
}