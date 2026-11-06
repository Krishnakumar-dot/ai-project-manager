using AiPmaPlatform.Domain.Entities.Timesheets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPmaPlatform.Infrastructure.Persistence.Configurations
{
    public class TimesheetEntryConfiguration : IEntityTypeConfiguration<TimesheetEntry>
    {
        public void Configure(EntityTypeBuilder<TimesheetEntry> builder)
        {
            builder.Property(e => e.Hours).HasPrecision(5, 2);
            builder.Property(e => e.Notes).HasMaxLength(500);

            // Restrict avoids SQL Server "multiple cascade paths" errors, and protects logged hours.
            builder.HasOne(e => e.Project)
                   .WithMany()
                   .HasForeignKey(e => e.ProjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ProjectTask)
                   .WithMany()
                   .HasForeignKey(e => e.ProjectTaskId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.TimesheetId);
        }
    }
}