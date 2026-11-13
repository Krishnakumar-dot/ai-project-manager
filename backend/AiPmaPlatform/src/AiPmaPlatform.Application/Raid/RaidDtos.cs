using AiPmaPlatform.Domain.Enums;

namespace AiPmaPlatform.Application.Raid
{
    public record RaidItemDto(
        Guid Id,
        Guid ProjectId,
        string ProjectName,
        RaidItemType Type,
        string Title,
        string? Description,
        int Impact,
        int Probability,
        int Score,
        string Level,
        RaidStatus Status,
        Guid? OwnerId,
        string? OwnerName,
        DateOnly? DueDate,
        string? MitigationPlan,
        string? RootCause,
        bool IsEscalated,
        string ReportedByName,
        DateTime CreatedAt,
        DateTime? ResolvedAt,
        bool IsOverdue);

    public record HeatMapCellDto(int Impact, int Probability, int Count);

    public record RaidSummaryDto(
        int ActiveTotal,
        int ActiveRisks,
        int ActiveAssumptions,
        int ActiveIssues,
        int ActiveDependencies,
        int CriticalCount,
        int HighCount,
        int OverdueCount,
        int EscalatedCount,
        List<HeatMapCellDto> HeatMap);
}