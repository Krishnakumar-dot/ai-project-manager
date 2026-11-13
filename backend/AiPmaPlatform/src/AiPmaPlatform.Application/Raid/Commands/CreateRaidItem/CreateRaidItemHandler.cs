using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Entities.Raid;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Raid.Commands.CreateRaidItem
{
    public class CreateRaidItemHandler : IRequestHandler<CreateRaidItemCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CreateRaidItemHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateRaidItemCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (userId is null)
                return ApiResponse<Guid>.Fail("You must be signed in.");

            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);
            if (!projectExists)
                return ApiResponse<Guid>.Fail("Project not found.");

            if (request.OwnerId.HasValue)
            {
                var ownerExists = await _context.Users
                    .AnyAsync(u => u.Id == request.OwnerId.Value, cancellationToken);
                if (!ownerExists)
                    return ApiResponse<Guid>.Fail("Owner not found.");
            }

            var item = new RaidItem
            {
                ProjectId = request.ProjectId,
                Type = request.Type,
                Title = request.Title.Trim(),
                Description = RaidRules.Clean(request.Description),
                Impact = request.Impact,
                Probability = request.Type == RaidItemType.Issue ? RaidRules.IssueProbability : request.Probability,
                Status = RaidStatus.Open,
                OwnerId = request.OwnerId,
                ReportedById = userId.Value,
                DueDate = request.DueDate,
                MitigationPlan = RaidRules.Clean(request.MitigationPlan),
                RootCause = RaidRules.Clean(request.RootCause)
            };

            _context.RaidItems.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(item.Id, $"{request.Type} logged successfully.");
        }
    }
}