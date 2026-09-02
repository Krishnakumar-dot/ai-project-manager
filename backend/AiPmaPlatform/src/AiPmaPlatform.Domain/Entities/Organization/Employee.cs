using AiPmaPlatform.Domain.Common;

namespace AiPmaPlatform.Domain.Entities.Organization
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}