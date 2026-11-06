using AiPmaPlatform.Domain.Entities.Timesheets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPmaPlatform.Infrastructure.Persistence.Configurations
{
    public class TimesheetConfiguration : IEntityTypeConfiguration<Timesheet>
    {
        public void Configure(EntityTypeBuilder<Timesheet> builder)
        {
            builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(t => t.ReviewComment).HasMaxLength(1000);

            // One timesheet per user per week.
            builder.HasIndex(t => new { t.UserId, t.WeekStartDate }).IsUnique();

            // Two foreign keys to User, so each is declared explicitly.
            builder.HasOne(t => t.User)
                   .WithMany()
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ReviewedBy)
                   .WithMany()
                   .HasForeignKey(t => t.ReviewedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Entries)
                   .WithOne(e => e.Timesheet)
                   .HasForeignKey(e => e.TimesheetId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}