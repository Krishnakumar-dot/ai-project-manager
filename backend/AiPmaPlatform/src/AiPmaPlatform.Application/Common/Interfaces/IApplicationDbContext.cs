using AiPmaPlatform.Domain.Entities.Identity;
using AiPmaPlatform.Domain.Entities.Organization;
using AiPmaPlatform.Domain.Entities.Portfolio;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<Department> Departments { get; }
        DbSet<Employee> Employees { get; }
        DbSet<User> Users { get; }
        DbSet<Portfolio> Portfolios { get; }
        DbSet<Project> Projects { get; }
        DbSet<Phase> Phases { get; }
        DbSet<Milestone> Milestones { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}