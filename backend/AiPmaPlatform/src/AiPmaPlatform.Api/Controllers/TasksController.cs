using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.ProjectTasks.Commands.CreateTask;
using AiPmaPlatform.Application.ProjectTasks.Commands.UpdateTaskStatus;
using AiPmaPlatform.Application.ProjectTasks.Queries.GetTasksByProject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TasksController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> Create(CreateTaskCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<ApiResponse<List<TaskDto>>>> GetByProject(Guid projectId)
            => Ok(await _mediator.Send(new GetTasksByProjectQuery { ProjectId = projectId }));

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse<Guid>>> UpdateStatus(Guid id, UpdateTaskStatusCommand command)
        {
            command.Id = id; // the route value wins over anything in the body
            return Ok(await _mediator.Send(command));
        }
    }
}