namespace AiPmaPlatform.Application.Financials
{
    public record ProjectInfo(
        Guid Id,
        string Name,
        string PortfolioName,
        decimal? Budget,
        decimal? BillingRate,
        DateTime? EndDate);

    // One row of approved timesheet hours, with the person's current cost rate.
    public record ApprovedHoursRow(
        Guid ProjectId,
        Guid UserId,
        string UserName,
        DateOnly WeekStart,
        decimal Hours,
        bool IsBillable,
        decimal? Rate);

    public static class FinancialMath
    {
        public const string StatusNoBudget = "NoBudget";
        public const string StatusOnTrack = "OnTrack";
        public const string StatusAtRisk = "AtRisk";
        public const string StatusOverBudget = "OverBudget";

        // Round half away from zero, so 7.25 becomes 7.3 (the default would give 7.2).
        public static decimal Round1(decimal value) => Math.Round(value, 1, MidpointRounding.AwayFromZero);
        public static decimal Round2(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        public static string BudgetStatus(decimal? budget, decimal cost)
        {
            if (!budget.HasValue || budget.Value <= 0m) return StatusNoBudget;

            var used = cost / budget.Value;
            if (used > 1m) return StatusOverBudget;
            if (used >= 0.8m) return StatusAtRisk;
            return StatusOnTrack;
        }

        public static ProjectFinancialSummaryDto BuildSummary(ProjectInfo project, IReadOnlyCollection<ApprovedHoursRow> rows)
        {
            var hours = rows.Sum(r => r.Hours);
            var billableHours = rows.Where(r => r.IsBillable).Sum(r => r.Hours);
            var cost = rows.Sum(r => r.Hours * (r.Rate ?? 0m));
            var hoursMissingRate = rows.Where(r => !r.Rate.HasValue).Sum(r => r.Hours);

            var revenue = project.BillingRate.HasValue ? billableHours * project.BillingRate.Value : 0m;
            var profit = revenue - cost;

            decimal? margin = revenue > 0m ? Round1(profit / revenue * 100m) : null;
            decimal? budgetUsed = project.Budget.HasValue && project.Budget.Value > 0m
                ? Round1(cost / project.Budget.Value * 100m)
                : null;

            return new ProjectFinancialSummaryDto(
                project.Id,
                project.Name,
                project.PortfolioName,
                project.Budget,
                project.BillingRate,
                Round2(cost),
                Round2(revenue),
                Round2(profit),
                margin,
                budgetUsed,
                Forecast(project, rows, cost),
                hours,
                billableHours,
                hoursMissingRate,
                BudgetStatus(project.Budget, cost));
        }

        // Simple forecast: cost so far + average weekly cost x weeks left until the project end date.
        // Needs an end date and at least one week of costed hours, otherwise it returns null.
        private static decimal? Forecast(ProjectInfo project, IReadOnlyCollection<ApprovedHoursRow> rows, decimal cost)
        {
            if (!project.EndDate.HasValue) return null;

            var weeksWithCost = rows
                .Where(r => (r.Rate ?? 0m) > 0m)
                .Select(r => r.WeekStart)
                .Distinct()
                .Count();
            if (weeksWithCost == 0) return null;

            var averageWeeklyCost = cost / weeksWithCost;
            var remainingDays = (project.EndDate.Value.Date - DateTime.UtcNow.Date).Days;
            var remainingWeeks = remainingDays > 0 ? (int)Math.Ceiling(remainingDays / 7.0) : 0;

            return Round2(cost + averageWeeklyCost * remainingWeeks);
        }
    }
}