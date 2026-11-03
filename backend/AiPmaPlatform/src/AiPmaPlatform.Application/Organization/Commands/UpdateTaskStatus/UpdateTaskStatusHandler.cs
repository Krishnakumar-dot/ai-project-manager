using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.ProjectTasks.Commands.UpdateTaskStatus
{
    public class UpdateTaskStatusHandler : IRequestHandler<UpdateTaskStatusCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public UpdateTaskStatusHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<Guid>> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.ProjectTasks
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (item is null)
                return ApiResponse<Guid>.Fail("Task not found.");

            item.Status = request.Status;
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(item.Id, "Task status updated.");
        }
    }
}