using AiPmaPlatform.Domain.Common;

namespace AiPmaPlatform.Domain.Entities.Portfolio
{
    public class Phase : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int SequenceOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }

        public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    }
}