using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Domain.Entities.Identity;
using AiPmaPlatform.Domain.Entities.Organization;
using AiPmaPlatform.Domain.Entities.Portfolio;
using AiPmaPlatform.Domain.Entities.ProjectTasks;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Portfolio> Portfolios => Set<Portfolio>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Phase> Phases => Set<Phase>();
        public DbSet<Milestone> Milestones => Set<Milestone>();
        public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
} 