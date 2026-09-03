using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Domain.Entities.Identity;
using MediatR;
using BCrypt.Net;

namespace AiPmaPlatform.Application.Identity.Commands.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public RegisterHandler(IApplicationDbContext context) => _context = context;

        public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Developer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
    }
}