using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.Financials;
using AiPmaPlatform.Application.Financials.Commands.UpdateProjectFinancials;
using AiPmaPlatform.Application.Financials.Commands.UpdateUserRate;
using AiPmaPlatform.Application.Financials.Queries.GetFinancialSummary;
using AiPmaPlatform.Application.Financials.Queries.GetProjectFinancials;
using AiPmaPlatform.Application.Financials.Queries.GetUserRates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FinancialsController : ControllerBase
    {
        private const string ViewRoles = "Admin,PMO,PM,Finance";
        private const string EditProjectRoles = "Admin,PMO,Finance";
        private const string RateRoles = "Admin,Finance";

        private readonly IMediator _mediator;
        public FinancialsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("summary")]
        [Authorize(Roles = ViewRoles)]
        public async Task<ActionResult<ApiResponse<List<ProjectFinancialSummaryDto>>>> GetSummary()
            => Ok(await _mediator.Send(new GetFinancialSummaryQuery()));

        [HttpGet("projects/{projectId}")]
        [Authorize(Roles = ViewRoles)]
        public async Task<ActionResult<ApiResponse<ProjectFinancialDetailDto>>> GetProject(Guid projectId)
            => Ok(await _mediator.Send(new GetProjectFinancialsQuery { ProjectId = projectId }));

        [HttpPut("projects/{projectId}")]
        [Authorize(Roles = EditProjectRoles)]
        public async Task<ActionResult<ApiResponse<Guid>>> UpdateProject(Guid projectId, UpdateProjectFinancialsCommand command)
        {
            command.ProjectId = projectId; // the route value wins over the body
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("rates")]
        [Authorize(Roles = RateRoles)]
        public async Task<ActionResult<ApiResponse<List<UserRateDto>>>> GetRates()
            => Ok(await _mediator.Send(new GetUserRatesQuery()));

        [HttpPut("rates/{userId}")]
        [Authorize(Roles = RateRoles)]
        public async Task<ActionResult<ApiResponse<Guid>>> UpdateRate(Guid userId, UpdateUserRateCommand command)
        {
            command.UserId = userId;
            return Ok(await _mediator.Send(command));
        }
    }
}