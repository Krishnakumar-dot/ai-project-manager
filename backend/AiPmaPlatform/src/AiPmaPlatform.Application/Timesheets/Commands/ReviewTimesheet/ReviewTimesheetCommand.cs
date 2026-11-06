using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Timesheets.Commands.ReviewTimesheet
{
    public class ReviewTimesheetCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
        public bool Approve { get; set; }
        public string? Comment { get; set; }
    }
}