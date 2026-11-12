using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Financials.Queries.GetUserRates
{
    public class GetUserRatesHandler : IRequestHandler<GetUserRatesQuery, ApiResponse<List<UserRateDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetUserRatesHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<List<UserRateDto>>> Handle(
            GetUserRatesQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .Select(u => new UserRateDto(u.Id, u.Name, u.Email, u.Role, u.HourlyCostRate))
                .ToListAsync(cancellationToken);

            return ApiResponse<List<UserRateDto>>.Success(users, "Rates retrieved successfully.");
        }
    }
}