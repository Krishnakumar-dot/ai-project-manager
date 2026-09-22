using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Entities.Portfolio;
using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreateProject
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateProjectHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                PortfolioId = request.PortfolioId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = ProjectStatus.NotStarted,
                OwnerId = _currentUserService.UserId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(project.Id, "Project created successfully.");
        }
    }
}