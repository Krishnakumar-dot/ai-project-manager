using AiPmaPlatform.Domain.Entities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPmaPlatform.Infrastructure.Persistence.Configurations
{
    public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
    {
        public void Configure(EntityTypeBuilder<Milestone> builder)
        {
            builder.Property(m => m.Name).IsRequired().HasMaxLength(200);

            builder.HasOne(m => m.Phase)
                   .WithMany(p => p.Milestones)
                   .HasForeignKey(m => m.PhaseId);
        }
    }
}