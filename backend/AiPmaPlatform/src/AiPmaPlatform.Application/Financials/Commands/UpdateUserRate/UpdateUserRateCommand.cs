using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Financials.Commands.UpdateUserRate
{
    public class UpdateUserRateCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid UserId { get; set; }

        // Null clears the rate.
        public decimal? HourlyCostRate { get; set; }
    }
}