using AiPmaPlatform.Domain.Common;

namespace AiPmaPlatform.Domain.Entities.Portfolio
{
    public class Milestone : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;

        public Guid PhaseId { get; set; }
        public Phase? Phase { get; set; }
    }
}