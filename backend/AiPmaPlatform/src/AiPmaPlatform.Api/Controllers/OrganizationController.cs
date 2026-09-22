using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.Organization.Commands.CreateDepartment;
using AiPmaPlatform.Application.Organization.Queries.GetEmployeeList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrganizationController(IMediator mediator) => _mediator = mediator;

        [HttpPost("departments")]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateDepartment(CreateDepartmentCommand command)
    => Ok(await _mediator.Send(command));

        [HttpGet("employees")]
        public async Task<ActionResult<ApiResponse<List<EmployeeDto>>>> GetEmployees()
            => Ok(await _mediator.Send(new GetEmployeeListQuery()));
    }
}