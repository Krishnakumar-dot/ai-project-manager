using MediatR;

namespace AiPmaPlatform.Application.Identity.Commands.Login
{
    public record LoginResult(string Token, string Name, string Role);

    public class LoginCommand : IRequest<LoginResult>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}