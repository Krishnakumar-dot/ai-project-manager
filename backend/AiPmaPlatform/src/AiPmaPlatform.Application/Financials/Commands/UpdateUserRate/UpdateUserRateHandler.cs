using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Financials.Commands.UpdateUserRate
{
    public class UpdateUserRateHandler : IRequestHandler<UpdateUserRateCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public UpdateUserRateHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<Guid>> Handle(UpdateUserRateCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
                return ApiResponse<Guid>.Fail("User not found.");

            user.HourlyCostRate = request.HourlyCostRate;
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(user.Id, $"Rate updated for {user.Name}.");
        }
    }
}