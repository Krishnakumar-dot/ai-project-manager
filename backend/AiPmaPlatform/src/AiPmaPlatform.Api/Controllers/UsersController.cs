using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.Identity.Commands.UpdateUserRole;
using AiPmaPlatform.Application.Identity.Queries.GetUsersList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserListDto>>>> GetAll()
            => Ok(await _mediator.Send(new GetUsersListQuery()));

        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Guid>>> UpdateRole(Guid id, UpdateUserRoleCommand command)
        {
            command.UserId = id; // the route value wins over the body
            return Ok(await _mediator.Send(command));
        }
    }
}