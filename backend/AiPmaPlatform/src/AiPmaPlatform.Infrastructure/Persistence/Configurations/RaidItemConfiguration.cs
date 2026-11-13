using AiPmaPlatform.Domain.Entities.Raid;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPmaPlatform.Infrastructure.Persistence.Configurations
{
    public class RaidItemConfiguration : IEntityTypeConfiguration<RaidItem>
    {
        public void Configure(EntityTypeBuilder<RaidItem> builder)
        {
            builder.Property(i => i.Title).IsRequired().HasMaxLength(200);
            builder.Property(i => i.Description).HasMaxLength(2000);
            builder.Property(i => i.MitigationPlan).HasMaxLength(2000);
            builder.Property(i => i.RootCause).HasMaxLength(2000);
            builder.Property(i => i.Type).HasConversion<string>().HasMaxLength(20);
            builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

            // Database-level safety net: impact and probability can never be stored outside 1-5.
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_RaidItems_Impact", "[Impact] BETWEEN 1 AND 5");
                t.HasCheckConstraint("CK_RaidItems_Probability", "[Probability] BETWEEN 1 AND 5");
            });

            builder.HasOne(i => i.Project)
                   .WithMany()
                   .HasForeignKey(i => i.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Two foreign keys to User, each declared explicitly and kept restrictive.
            builder.HasOne(i => i.Owner)
                   .WithMany()
                   .HasForeignKey(i => i.OwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.ReportedBy)
                   .WithMany()
                   .HasForeignKey(i => i.ReportedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => i.ProjectId);
            builder.HasIndex(i => i.Status);
        }
    }
}