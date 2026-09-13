using AiPmaPlatform.Domain.Common;
using AiPmaPlatform.Domain.Enums;

namespace AiPmaPlatform.Domain.Entities.Portfolio
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid PortfolioId { get; set; }
        public Portfolio? Portfolio { get; set; }

        public Guid? OwnerId { get; set; } // links to User (PM)

        public ICollection<Phase> Phases { get; set; } = new List<Phase>();
    }
}