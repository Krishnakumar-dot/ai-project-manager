using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Entities.ProjectTasks;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.ProjectTasks.Commands.CreateTask
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public CreateTaskHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);
            if (!projectExists)
                return ApiResponse<Guid>.Fail("Project not found.");

            if (request.AssigneeId.HasValue)
            {
                var userExists = await _context.Users
                    .AnyAsync(u => u.Id == request.AssigneeId.Value, cancellationToken);
                if (!userExists)
                    return ApiResponse<Guid>.Fail("Assignee not found.");
            }

            var item = new ProjectTask
            {
                Title = request.Title,
                Description = request.Description,
                ProjectId = request.ProjectId,
                Priority = request.Priority,
                DueDate = request.DueDate,
                AssigneeId = request.AssigneeId,
                Status = ProjectTaskStatus.ToDo
            };

            _context.ProjectTasks.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(item.Id, "Task created successfully.");
        }
    }
}