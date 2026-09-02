using AiPmaPlatform.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<Department> Departments { get; }
        DbSet<Employee> Employees { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}