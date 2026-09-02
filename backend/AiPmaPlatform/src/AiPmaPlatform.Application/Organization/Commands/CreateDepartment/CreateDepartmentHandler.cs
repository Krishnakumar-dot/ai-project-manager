using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Domain.Entities.Organization;
using MediatR;

namespace AiPmaPlatform.Application.Organization.Commands.CreateDepartment
{
    public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateDepartmentHandler(IApplicationDbContext context) => _context = context;

        public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = new Department
            {
                Name = request.Name,
                CompanyId = request.CompanyId
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);

            return department.Id;
        }
    }
}