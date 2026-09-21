using AiPmaPlatform.Application.Identity.Commands.Login;
using AiPmaPlatform.Application.Identity.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AiPmaPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("register")]
        public async Task<ActionResult<Guid>> Register(RegisterCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPost("login")]
        public async Task<ActionResult<LoginResult>> Login(LoginCommand command)
            => Ok(await _mediator.Send(command));
    }
}