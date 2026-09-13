using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Queries.GetProjectsList
{
    public record ProjectListDto(Guid Id, string Name, ProjectStatus Status, DateTime StartDate, DateTime? EndDate, string PortfolioName);

    public class GetProjectsListQuery : IRequest<List<ProjectListDto>> { }
}