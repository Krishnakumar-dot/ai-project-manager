using AiPmaPlatform.Application.Common.Interfaces;
using AiPmaPlatform.Application.Common.Models;
using AiPmaPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiPmaPlatform.Application.Timesheets.Commands.ReviewTimesheet
{
    public class ReviewTimesheetHandler : IRequestHandler<ReviewTimesheetCommand, ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public ReviewTimesheetHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<Guid>> Handle(ReviewTimesheetCommand request, CancellationToken cancellationToken)
        {
            var reviewerId = _currentUser.UserId;
            if (reviewerId is null)
                return ApiResponse<Guid>.Fail("You must be signed in.");

            var sheet = await _context.Timesheets
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (sheet is null)
                return ApiResponse<Guid>.Fail("Timesheet not found.");

            if (sheet.UserId == reviewerId.Value)
                return ApiResponse<Guid>.Fail("You can't review your own timesheet.");

            if (sheet.Status != TimesheetStatus.Submitted)
                return ApiResponse<Guid>.Fail("Only submitted timesheets can be reviewed.");

            sheet.Status = request.Approve ? TimesheetStatus.Approved : TimesheetStatus.Rejected;
            sheet.ReviewedById = reviewerId.Value;
            sheet.ReviewedAt = DateTime.UtcNow;
            sheet.ReviewComment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Guid>.Success(sheet.Id,
                request.Approve ? "Timesheet approved." : "Timesheet rejected.");
        }
    }
}