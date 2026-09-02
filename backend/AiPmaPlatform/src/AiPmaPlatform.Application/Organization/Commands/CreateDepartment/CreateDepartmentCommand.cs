using MediatR;

namespace AiPmaPlatform.Application.Organization.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
    }
}