using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreatePhase
{
    public class CreatePhaseCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public int SequenceOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}