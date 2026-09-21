using AiPmaPlatform.Domain.Common;

namespace AiPmaPlatform.Domain.Entities.Portfolio
{
    public class Portfolio : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}