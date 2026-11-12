namespace AiPmaPlatform.Application.Financials
{
    public record ProjectFinancialSummaryDto(
        Guid ProjectId,
        string ProjectName,
        string PortfolioName,
        decimal? Budget,
        decimal? BillingRatePerHour,
        decimal ActualCost,
        decimal Revenue,
        decimal Profit,
        decimal? MarginPercent,
        decimal? BudgetUsedPercent,
        decimal? ForecastCost,
        decimal ApprovedHours,
        decimal BillableHours,
        decimal HoursMissingRate,
        string BudgetStatus);

    public record CostByPersonDto(
        Guid UserId,
        string UserName,
        decimal Hours,
        decimal BillableHours,
        decimal? HourlyCostRate,
        decimal Cost);

    public record WeeklyCostDto(DateOnly WeekStart, decimal Cost, decimal Revenue);

    public record ProjectFinancialDetailDto(
        ProjectFinancialSummaryDto Summary,
        List<CostByPersonDto> ByPerson,
        List<WeeklyCostDto> ByWeek);

    public record UserRateDto(Guid Id, string Name, string Email, string Role, decimal? HourlyCostRate);
}