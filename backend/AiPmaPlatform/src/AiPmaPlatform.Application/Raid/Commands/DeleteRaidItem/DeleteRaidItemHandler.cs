using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Raid.Commands.DeleteRaidItem
{
    public class DeleteRaidItemHandler : IRequestHandler<DeleteRaidItemCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteRaidItemHandler(IApplicationDbContext context) => _context = context;

        public async Task<ApiResponse<Guid>> Handle(DeleteRaidItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.RaidItems
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            if (item is null)
                return ApiResponse<Guid>.Fail("Item not found.");

            _context.RaidItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(request.Id, "Item deleted.");
        }
    }
}