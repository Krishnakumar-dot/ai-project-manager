using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Queries.GetProjectById
{
    public record PhaseDto(Guid Id, string Name, int SequenceOrder);
    public record ProjectDetailDto(Guid Id, string Name, string? Description, string Status,
        DateTime StartDate, DateTime? EndDate, List<PhaseDto> Phases);

    public class GetProjectByIdQuery : IRequest<ProjectDetailDto?>
    {
        public Guid Id { get; set; }
    }
}