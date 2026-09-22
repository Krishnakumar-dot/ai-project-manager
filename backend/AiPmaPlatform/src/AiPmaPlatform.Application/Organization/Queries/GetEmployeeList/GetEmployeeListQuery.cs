using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Organization.Queries.GetEmployeeList
{
    public record EmployeeDto(Guid Id, string Name, string Email, string DepartmentName);

    public class GetEmployeeListQuery : IRequest<ApiResponse<List<EmployeeDto>>> { }
}