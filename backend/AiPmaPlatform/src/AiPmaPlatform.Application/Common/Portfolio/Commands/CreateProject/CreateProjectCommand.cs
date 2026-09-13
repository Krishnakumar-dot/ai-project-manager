using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Commands.CreateProject
{
    public class CreateProjectCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid PortfolioId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}