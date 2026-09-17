using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Domain.Entities.Portfolio;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreateMilestone
{
    public class CreateMilestoneHandler : IRequestHandler<CreateMilestoneCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        public CreateMilestoneHandler(IApplicationDbContext context) => _context = context;

        public async Task<Guid> Handle(CreateMilestoneCommand request, CancellationToken cancellationToken)
        {
            var milestone = new Milestone
            {
                Name = request.Name,
                PhaseId = request.PhaseId,
                DueDate = request.DueDate,
                IsCompleted = false
            };

            _context.Milestones.Add(milestone);
            await _context.SaveChangesAsync(cancellationToken);
            return milestone.Id;
        }
    }
}