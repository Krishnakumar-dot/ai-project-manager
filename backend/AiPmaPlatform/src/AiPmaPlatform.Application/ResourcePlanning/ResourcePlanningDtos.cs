namespace AiPmaPlatform.Application.ResourcePlanning
{
    public record AllocationDto(
        Guid Id,
        Guid UserId,
        string UserName,
        Guid ProjectId,
        string ProjectName,
        int AllocationPercent,
        DateOnly StartDate,
        DateOnly EndDate,
        string? RoleOnProject);

    public record UtilizationDto(
        Guid UserId,
        string UserName,
        int AllocatedPercent,
        decimal CapacityHours,
        decimal PlannedHours,
        decimal LoggedHours,
        string Status,
        List<string> Projects);
}