using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;

namespace AiPmaPlatform.Application.Portfolio.Queries.GetProjectsList
{
    public record ProjectListDto(Guid Id, string Name, ProjectStatus Status, DateTime StartDate, DateTime? EndDate, string PortfolioName);

    public class GetProjectsListQuery : IRequest<ApiResponse<List<ProjectListDto>>> { }
}