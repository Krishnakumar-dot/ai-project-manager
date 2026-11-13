using AiPmaPlatform.Application.Common.Constants;
using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Raid.Commands.UpdateRaidItem
{
    public class UpdateRaidItemHandler : IRequestHandler<UpdateRaidItemCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateRaidItemHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<Guid>> Handle(UpdateRaidItemCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (userId is null)
                return ApiResponse<Guid>.Fail("You must be signed in.");

            var item = await _context.RaidItems
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            if (item is null)
                return ApiResponse<Guid>.Fail("Item not found.");

            var isManager = AppRoles.Managers.Contains(_currentUser.Role ?? string.Empty);
            if (!isManager && item.OwnerId != userId && item.ReportedById != userId)
                return ApiResponse<Guid>.Fail("Only the owner, the reporter, or a manager can edit this item.");

            if (request.OwnerId.HasValue)
            {
                var ownerExists = await _context.Users
                    .AnyAsync(u => u.Id == request.OwnerId.Value, cancellationToken);
                if (!ownerExists)
                    return ApiResponse<Guid>.Fail("Owner not found.");
            }

            item.Title = request.Title.Trim();
            item.Description = RaidRules.Clean(request.Description);
            item.Impact = request.Impact;
            item.Probability = item.Type == RaidItemType.Issue ? RaidRules.IssueProbability : request.Probability;
            item.OwnerId = request.OwnerId;
            item.DueDate = request.DueDate;
            item.MitigationPlan = RaidRules.Clean(request.MitigationPlan);
            item.RootCause = RaidRules.Clean(request.RootCause);

            // Track when an item was resolved. Reopening it clears the date again.
            item.Status = request.Status;
            if (RaidRules.IsActive(request.Status))
                item.ResolvedAt = null;
            else if (item.ResolvedAt is null)
                item.ResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(item.Id, "Item updated.");
        }
    }
}