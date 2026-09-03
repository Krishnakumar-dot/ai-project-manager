using AiPmaPlatform.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPmaPlatform.Infrastructure.Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.Property(d => d.Name).IsRequired().HasMaxLength(200);

            builder.HasOne(d => d.Company)
                   .WithMany(c => c.Departments)
                   .HasForeignKey(d => d.CompanyId);
        }
    }
}