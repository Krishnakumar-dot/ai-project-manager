using MediatR;

namespace AiPmaPlatform.Application.Identity.Commands.Register
{
    public class RegisterCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}