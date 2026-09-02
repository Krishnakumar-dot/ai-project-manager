using AiPmaPlatform.Domain.Common;

namespace AiPmaPlatform.Domain.Entities.Organization
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}