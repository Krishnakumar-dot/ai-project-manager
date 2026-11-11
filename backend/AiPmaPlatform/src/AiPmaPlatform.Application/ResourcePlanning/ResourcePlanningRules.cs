namespace AiPmaPlatform.Application.ResourcePlanning
{
    public static class ResourcePlanningRules
    {
        // Standard working week. Later this can come from the Employee record instead.
        public const decimal WeeklyCapacityHours = 40m;

        public const string StatusBench = "Bench";
        public const string StatusAllocated = "Allocated";
        public const string StatusOverallocated = "Overallocated";
    }
}