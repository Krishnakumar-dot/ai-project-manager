using AiPmaPlatform.Application.Portfolio.Commands.CreateProject;
using AiPmaPlatform.Application.Portfolio.Queries.GetProjectById;
using AiPmaPlatform.Application.Portfolio.Queries.GetProjectsList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProjectsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateProjectCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet]
        public async Task<ActionResult<List<ProjectListDto>>> GetAll()
            => Ok(await _mediator.Send(new GetProjectsListQuery()));

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDetailDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProjectByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }
    }
}