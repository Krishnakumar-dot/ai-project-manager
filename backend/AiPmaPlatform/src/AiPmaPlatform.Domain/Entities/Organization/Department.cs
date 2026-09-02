using AiPmaPlatform.Domain.Common;

namespace AiPmaPlatform.Domain.Entities.Organization
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public Guid CompanyId { get; set; }
        public Company? Company { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}