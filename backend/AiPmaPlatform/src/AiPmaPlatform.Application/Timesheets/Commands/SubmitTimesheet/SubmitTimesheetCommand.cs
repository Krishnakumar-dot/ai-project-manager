using AiPmaPlatform.Application.Common.Models;
using MediatR;

namespace AiPmaPlatform.Application.Timesheets.Commands.SubmitTimesheet
{
    public class SubmitTimesheetCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
    }
}