using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Identity.Queries.GetUsersList
{
    public record UserListDto(Guid Id, string Name, string Email, string Role);

    public class GetUsersListQuery : IRequest<ApiResponse<List<UserListDto>>> { }
}