using AiPmaPlatform.Domain.Common;
using AiPmaPlatform.Domain.Entities.Identity;
using AiPmaPlatform.Domain.Entities.Portfolio;
using AiPmaPlatform.Domain.Enums;

namespace AiPmaPlatform.Domain.Entities.Raid
{
    public class RaidItem : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }

        public RaidItemType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Both 1 to 5. The score is calculated when read, never stored, so it can't go stale.
        public int Impact { get; set; }
        public int Probability { get; set; }

        public RaidStatus Status { get; set; } = RaidStatus.Open;

        public Guid? OwnerId { get; set; }
        public User? Owner { get; set; }

        public Guid ReportedById { get; set; }
        public User? ReportedBy { get; set; }

        public DateOnly? DueDate { get; set; }
        public string? MitigationPlan { get; set; }
        public string? RootCause { get; set; }

        public bool IsEscalated { get; set; }
        public DateTime? EscalatedAt { get; set; }

        // Set when the item moves to Resolved or Closed, cleared if it is reopened.
        public DateTime? ResolvedAt { get; set; }
    }
}