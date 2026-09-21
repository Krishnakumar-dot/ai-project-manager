using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Identity.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResult>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginHandler(IApplicationDbContext context, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<ApiResponse<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return ApiResponse<LoginResult>.Fail("Invalid email or password.");

            var token = _jwtTokenService.GenerateToken(user);
            var result = new LoginResult(token, user.Name, user.Role);

            return ApiResponse<LoginResult>.Success(result, "Login successful.");
        }
    }
}