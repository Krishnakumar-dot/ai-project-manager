using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Financials.Commands.UpdateProjectFinancials
{
    public class UpdateProjectFinancialsCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid ProjectId { get; set; }

        // Null clears the value.
        public decimal? Budget { get; set; }
        public decimal? BillingRatePerHour { get; set; }
    }
}