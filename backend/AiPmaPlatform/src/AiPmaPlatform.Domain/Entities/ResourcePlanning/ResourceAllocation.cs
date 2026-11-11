using AiPmaPlatform.Domain.Common;
using AiPmaPlatform.Domain.Entities.Identity;
using AiPmaPlatform.Domain.Entities.Portfolio;

namespace AiPmaPlatform.Domain.Entities.ResourcePlanning
{
    public class ResourceAllocation : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }

        // Share of the person's working week, 1 to 100.
        public int AllocationPercent { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        // Free text such as "Backend Developer". Not the same as the login role (Admin, PM...).
        public string? RoleOnProject { get; set; }
    }
}