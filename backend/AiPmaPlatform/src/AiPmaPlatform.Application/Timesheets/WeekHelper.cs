namespace AiPmaPlatform.Application.Timesheets
{
    public static class WeekHelper
    {
        public static DateOnly StartOfWeek(DateOnly date)
        {
            var diff = ((int)date.DayOfWeek + 6) % 7; // Monday = 0 ... Sunday = 6
            return date.AddDays(-diff);
        }
    }
}