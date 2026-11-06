using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Timesheets.Commands.SubmitTimesheet
{
    public class SubmitTimesheetHandler : IRequestHandler<SubmitTimesheetCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public SubmitTimesheetHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<Guid>> Handle(SubmitTimesheetCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (userId is null)
                return ApiResponse<Guid>.Fail("You must be signed in.");

            var sheet = await _context.Timesheets
                .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == userId.Value, cancellationToken);

            if (sheet is null)
                return ApiResponse<Guid>.Fail("Timesheet not found.");

            if (sheet.Status is not (TimesheetStatus.Draft or TimesheetStatus.Rejected))
                return ApiResponse<Guid>.Fail("Only draft or rejected timesheets can be submitted.");

            var hasHours = await _context.TimesheetEntries
                .AnyAsync(e => e.TimesheetId == sheet.Id && e.Hours > 0, cancellationToken);
            if (!hasHours)
                return ApiResponse<Guid>.Fail("Add at least one entry before submitting.");

            sheet.Status = TimesheetStatus.Submitted;
            sheet.SubmittedAt = DateTime.UtcNow;
            sheet.ReviewedById = null;
            sheet.ReviewedAt = null;
            sheet.ReviewComment = null;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(sheet.Id, "Timesheet submitted for approval.");
        }
    }
}