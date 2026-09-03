using AiPmaPlatform.Domain.Entities.Identity;
using AiPmaPlatform.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<Department> Departments { get; }
        DbSet<Employee> Employees { get; }
        DbSet<User> Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}