using AiPmaPlatform.Application.Common.Constants;
using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Raid.Commands.SetRaidEscalation
{
    public class SetRaidEscalationHandler : IRequestHandler<SetRaidEscalationCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public SetRaidEscalationHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<Guid>> Handle(SetRaidEscalationCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.RaidItems
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            if (item is null)
                return ApiResponse<Guid>.Fail("Item not found.");

            // Anyone can raise an escalation, but only a manager can take it back.
            var isManager = AppRoles.Managers.Contains(_currentUser.Role ?? string.Empty);
            if (!request.Escalate && !isManager)
                return ApiResponse<Guid>.Fail("Only a manager can remove an escalation.");

            item.IsEscalated = request.Escalate;
            item.EscalatedAt = request.Escalate ? DateTime.UtcNow : null;
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(item.Id, request.Escalate ? "Item escalated." : "Escalation removed.");
        }
    }
}