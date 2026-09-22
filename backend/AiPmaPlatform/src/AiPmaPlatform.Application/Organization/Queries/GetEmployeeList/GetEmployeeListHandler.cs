using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Organization.Queries.GetEmployeeList
{
    public class GetEmployeeListHandler : IRequestHandler<GetEmployeeListQuery, ApiResponse<List<EmployeeDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetEmployeeListHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<EmployeeDto>>> Handle(GetEmployeeListQuery request, CancellationToken cancellationToken)
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Select(e => new EmployeeDto(e.Id, e.Name, e.Email, e.Department!.Name))
                .ToListAsync(cancellationToken);

            return ApiResponse<List<EmployeeDto>>.Success(employees, "Employees retrieved successfully.");
        }
    }
}