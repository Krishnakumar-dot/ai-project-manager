using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Application.Timesheets;
using AiPmaPlatform.Domain.Entities.ResourcePlanning;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.ResourcePlanning.Commands.CreateAllocation
{
    public class CreateAllocationHandler : IRequestHandler<CreateAllocationCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public CreateAllocationHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<Guid>> Handle(CreateAllocationCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == request.UserId, cancellationToken);
            if (!userExists)
                return ApiResponse<Guid>.Fail("User not found.");

            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);
            if (!projectExists)
                return ApiResponse<Guid>.Fail("Project not found.");

            if (!request.AllowOverallocation)
            {
                // Only this person's allocations that overlap the new date range matter.
                var existing = await _context.ResourceAllocations
                    .AsNoTracking()
                    .Where(a => a.UserId == request.UserId
                             && a.StartDate <= request.EndDate
                             && a.EndDate >= request.StartDate)
                    .Select(a => new { a.AllocationPercent, a.StartDate, a.EndDate })
                    .ToListAsync(cancellationToken);

                // Checked week by week (Monday to Sunday). Allocations touching the same
                // week count together, which is the safe side for planning.
                for (var weekStart = WeekHelper.StartOfWeek(request.StartDate);
                     weekStart <= request.EndDate;
                     weekStart = weekStart.AddDays(7))
                {
                    var weekEnd = weekStart.AddDays(6);
                    var current = existing
                        .Where(a => a.StartDate <= weekEnd && a.EndDate >= weekStart)
                        .Sum(a => a.AllocationPercent);

                    if (current + request.AllocationPercent > 100)
                    {
                        return ApiResponse<Guid>.Fail(
                            $"This would allocate the person to {current + request.AllocationPercent}% " +
                            $"in the week of {weekStart:dd MMM yyyy}. They already have {current}% allocated.");
                    }
                }
            }

            var allocation = new ResourceAllocation
            {
                UserId = request.UserId,
                ProjectId = request.ProjectId,
                AllocationPercent = request.AllocationPercent,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RoleOnProject = string.IsNullOrWhiteSpace(request.RoleOnProject)
                    ? null
                    : request.RoleOnProject.Trim()
            };

            _context.ResourceAllocations.Add(allocation);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(allocation.Id, "Allocation created successfully.");
        }
    }
}