using AiPmaPlatform.Domain.Entities.ResourcePlanning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPmaPlatform.Infrastructure.Persistence.Configurations
{
    public class ResourceAllocationConfiguration : IEntityTypeConfiguration<ResourceAllocation>
    {
        public void Configure(EntityTypeBuilder<ResourceAllocation> builder)
        {
            builder.Property(a => a.RoleOnProject).HasMaxLength(100);

            // Database-level safety net: the percentage can never be stored outside 1-100.
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_ResourceAllocations_Percent",
                "[AllocationPercent] BETWEEN 1 AND 100"));

            builder.HasOne(a => a.User)
                   .WithMany()
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Project)
                   .WithMany()
                   .HasForeignKey(a => a.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.ProjectId);
        }
    }
}