using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.ProjectTasks.Queries.GetTasksByProject
{
    public class GetTasksByProjectHandler
        : IRequestHandler<GetTasksByProjectQuery, ApiResponse<List<TaskDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetTasksByProjectHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<TaskDto>>> Handle(
            GetTasksByProjectQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == request.ProjectId)
                .OrderBy(t => t.CreatedAt)
                .Select(t => new TaskDto(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.Priority,
                    t.DueDate,
                    t.AssigneeId,
                    t.Assignee != null ? t.Assignee.Name : null))
                .ToListAsync(cancellationToken);

            return ApiResponse<List<TaskDto>>.Success(tasks, "Tasks retrieved successfully.");
        }
    }
}