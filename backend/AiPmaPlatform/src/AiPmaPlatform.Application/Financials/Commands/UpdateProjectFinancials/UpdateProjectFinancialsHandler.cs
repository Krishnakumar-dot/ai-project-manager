using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Financials.Commands.UpdateProjectFinancials
{
    public class UpdateProjectFinancialsHandler
        : IRequestHandler<UpdateProjectFinancialsCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public UpdateProjectFinancialsHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<Guid>> Handle(
            UpdateProjectFinancialsCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (project is null)
                return ApiResponse<Guid>.Fail("Project not found.");

            project.Budget = request.Budget;
            project.BillingRatePerHour = request.BillingRatePerHour;
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(project.Id, "Project financials updated.");
        }
    }
}