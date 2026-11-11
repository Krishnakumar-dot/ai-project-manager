using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.ResourcePlanning.Commands.DeleteAllocation
{
    public class DeleteAllocationHandler : IRequestHandler<DeleteAllocationCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteAllocationHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<Guid>> Handle(DeleteAllocationCommand request, CancellationToken cancellationToken)
        {
            var allocation = await _context.ResourceAllocations
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (allocation is null)
                return ApiResponse<Guid>.Fail("Allocation not found.");

            _context.ResourceAllocations.Remove(allocation);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(request.Id, "Allocation removed.");
        }
    }
}