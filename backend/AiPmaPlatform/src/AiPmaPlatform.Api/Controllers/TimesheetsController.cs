using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.Timesheets;
using AiPmaPlatform.Application.Timesheets.Commands.ReviewTimesheet;
using AiPmaPlatform.Application.Timesheets.Commands.SaveTimesheet;
using AiPmaPlatform.Application.Timesheets.Commands.SubmitTimesheet;
using AiPmaPlatform.Application.Timesheets.Queries.GetMyTimesheetWeek;
using AiPmaPlatform.Application.Timesheets.Queries.GetPendingTimesheets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TimesheetsController : ControllerBase
    {
        private const string ApproverRoles = "Admin,PM,PMO";

        private readonly IMediator _mediator;
        public TimesheetsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("week")]
        public async Task<ActionResult<ApiResponse<TimesheetDto>>> GetWeek([FromQuery] DateOnly weekStart)
            => Ok(await _mediator.Send(new GetMyTimesheetWeekQuery { WeekStart = weekStart }));

        [HttpPut("week")]
        public async Task<ActionResult<ApiResponse<Guid>>> Save(SaveTimesheetCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPost("{id}/submit")]
        public async Task<ActionResult<ApiResponse<Guid>>> Submit(Guid id)
            => Ok(await _mediator.Send(new SubmitTimesheetCommand { Id = id }));

        [HttpGet("pending")]
        [Authorize(Roles = ApproverRoles)]
        public async Task<ActionResult<ApiResponse<List<PendingTimesheetDto>>>> GetPending()
            => Ok(await _mediator.Send(new GetPendingTimesheetsQuery()));

        [HttpPost("{id}/review")]
        [Authorize(Roles = ApproverRoles)]
        public async Task<ActionResult<ApiResponse<Guid>>> Review(Guid id, ReviewTimesheetCommand command)
        {
            command.Id = id; // route value wins over the body
            return Ok(await _mediator.Send(command));
        }
    }
}