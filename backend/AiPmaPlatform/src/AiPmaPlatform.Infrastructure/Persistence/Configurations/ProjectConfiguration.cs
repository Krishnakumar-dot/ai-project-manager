using AiPmaPlatform.Domain.Entities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPmaPlatform.Infrastructure.Persistence.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(p => p.Portfolio)
                   .WithMany(pf => pf.Projects)
                   .HasForeignKey(p => p.PortfolioId);
        }
    }
}