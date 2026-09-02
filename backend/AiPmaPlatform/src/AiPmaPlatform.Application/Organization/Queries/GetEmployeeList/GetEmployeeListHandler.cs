using AiPmaPlatform.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Organization.Queries.GetEmployeeList
{
    public class GetEmployeeListHandler : IRequestHandler<GetEmployeeListQuery, List<EmployeeDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetEmployeeListHandler(IApplicationDbContext context) => _context = context;

        public async Task<List<EmployeeDto>> Handle(GetEmployeeListQuery request, CancellationToken cancellationToken)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Select(e => new EmployeeDto(e.Id, e.Name, e.Email, e.Department!.Name))
                .ToListAsync(cancellationToken);
        }
    }
}