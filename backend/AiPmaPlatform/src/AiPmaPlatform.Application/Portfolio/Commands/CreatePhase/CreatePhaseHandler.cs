using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Domain.Entities.Portfolio;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreatePhase
{
    public class CreatePhaseHandler : IRequestHandler<CreatePhaseCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        public CreatePhaseHandler(IApplicationDbContext context) => _context = context;

        public async Task<Guid> Handle(CreatePhaseCommand request, CancellationToken cancellationToken)
        {
            var phase = new Phase
            {
                Name = request.Name,
                ProjectId = request.ProjectId,
                SequenceOrder = request.SequenceOrder,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            _context.Phases.Add(phase);
            await _context.SaveChangesAsync(cancellationToken);
            return phase.Id;
        }
    }
}