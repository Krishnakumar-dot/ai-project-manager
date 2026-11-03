using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Identity.Queries.GetUsersList
{
    public class GetUsersListHandler : IRequestHandler<GetUsersListQuery, ApiResponse<List<UserListDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetUsersListHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<UserListDto>>> Handle(
            GetUsersListQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .OrderBy(u => u.Name)
                .Select(u => new UserListDto(u.Id, u.Name, u.Email, u.Role))
                .ToListAsync(cancellationToken);

            return ApiResponse<List<UserListDto>>.Success(users, "Users retrieved successfully.");
        }
    }
}