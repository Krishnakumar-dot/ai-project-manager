using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Identity.Commands.UpdateUserRole
{
    public class UpdateUserRoleHandler : IRequestHandler<UpdateUserRoleCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateUserRoleHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<Guid>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            // Stops the last Admin from locking everyone out by demoting themselves.
            if (_currentUser.UserId == request.UserId)
                return ApiResponse<Guid>.Fail("You can't change your own role.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
                return ApiResponse<Guid>.Fail("User not found.");

            user.Role = request.Role;
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(user.Id, $"{user.Name} is now {request.Role}.");
        }
    }
}