using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Identity.Commands.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public RegisterHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

            if (exists)
                return ApiResponse<Guid>.Fail("An account with this email already exists.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Developer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(user.Id, "Account created successfully.");
        }
    }
}